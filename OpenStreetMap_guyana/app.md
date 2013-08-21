Draw an OpenStreetMap with processing
=====================================

The goal of this article is to display data from the open cartography project OpenStreetMap. Only the street will be displayed with simple lines.

OpenStreetMap map are xml files. We will use Processing as programming tool. It's able to load xml files and draw very easily. The language behind the scene is java.

All coordinates are in longitude/lattitude unit, we need first to convert it into a 2D plan in meters. It's an early approximation, that will change distance between two far points but will be a good approximation in the same small country. It will let easily see if the order of magnitude of the distance displayed is good. 

Then we will load the xml file in memory. And finally we will draw map on the screen.

For this example I use a map of Guyana, because it's one of the smaller map available. You can download it from here:

[http://download.geofabrik.de/europe/france/guyane.html]()

You must use the non compressed format `.osm`.

Convert latitude/longitude to meter
-----------------------------------

```
	<<constants>> =
		static int kRadiusEarth_m = 6373000;
```

The conversion take a longitude / latitude to convert and the origin in longitude / latitude.

```
	<<convert>> =
		static public PVector ConvertLatLongToMeters(float lon, float lat, float lon_origin, float lat_origin) {
			PVector v = new PVector(0.0f, 0.0f);
			float dLat = lat - lat_origin;
			float dLon = lon - lon_origin;
			v.x = dLon * TWO_PI / 360.0f * kRadiusEarth_m;
			v.y = dLat * TWO_PI / 360.0f * kRadiusEarth_m;
			return v;
		}
```

If you want more information see:
[http://www.johndcook.com/lat_long_details.html]()

Load XML file
-------------

We use the loadXML method from processing: [http://processing.org/reference/loadXML_.html]()

```
	<<load-file>> =
		XML xml = loadXML(filename);

		<<parse-bound>>

		<<parse-nodes>>

		<<parse-ways>>
```

The xml file will be as this one (I clean not used properties). You must not use not compressed format `.osm`:

```
<?xml version='1.0' encoding='UTF-8'?>
<osm>
	<bounds minlat="5.3" minlon="54.0" maxlat="5.4" maxlon="54.2"/>
	<node id="1620763534" lat="5.3" lon="54.2"/>
	<node id="1620763530" lat="5.4" lon="54.1"/>
	<node id="1620763610" lat="5.3" lon="54.0"/>
	<way>
		<nd ref="1620763534"/>
		<nd ref="1620763530"/>
		<nd ref="1620763610"/>
		<tag k="maxspeed" v="50"/>
		<tag k="width" v="8"/>
	</way>
</osm>

```

We see that first the file create nodes that are point with a coordinate and reference after through his id. Then we see that way are array of node with some properties.

As we say before, we will convert lon/lat into meters. For this operation we need the bounding box to take an origin.

```
	<<parse-bound>> =
		XML[] boundNodes = xml.getChildren("bounds");
		if(boundNodes.length == 0) {
			println("error: bounds in not present in the xml file");
			return;
		}
		XML b = boundNodes[0];
		minlat = b.getFloat("minlat");
		minlon = b.getFloat("minlon");
		maxlat = b.getFloat("maxlat");
		maxlon = b.getFloat("maxlon");

		PVector vMax = ConvertLatLongToMeters(maxlon, maxlat, minlon, minlat);
		maxX = vMax.x;
		maxY = vMax.y;
```

We first parse the nodes.

```
	<<parse-nodes>> =
		XML[] nodes = xml.getChildren("node");
		for (int i = 0; i < nodes.length; i++) {         
  			XML n = nodes[i];
  			<<convert-node>>
		}
```

The coordinate of the node is in long/lat fromat, we will first convert it in meters using min bounds as origin.

```
	<<convert-node>> = 
		float lat = n.getFloat("lat");
		float lon = n.getFloat("lon");
		String id = n.getString("id");
		PVector v = ConvertLatLongToMeters(lon, lat, minlon, minlat);
		<<push-node>>
```

As further in the code we will need to access it by it's id, we insert it in a hashMap.

```
	<<push-node>> =
		nodeMap.put(id, v);
```

Now that we have nodes, we create ways.

```
	<<parse-ways>> =
		XML[] ways = xml.getChildren("way");
		wayArray = new Way[ways.length];
		for (int j = 0; j < ways.length; j++) {         
  			XML wNode = ways[j];

  			Way w = new Way();

  			<<extract-tags>> 

  			<<convert-way>>

  			wayArray[j] = w;
		}
```

We can first search for tags.

```
	<<extract-tags>> =
		XML[] tags = wNode.getChildren("tag");
		for (int k = 0; k < tags.length; k++) {         
  			XML t = tags[k];
  			String tName = t.getString("k");
  			if(tName == "width") {
  				w.width = t.getInt("v");
  			}
  			if(tName == "maxspeed") {
  				w.maxSpeed = t.getInt("v");
  			}
		}
```

Then we add the node list into the way.

```
	<<convert-way>> =
		XML[] nodeRefArray = wNode.getChildren("nd");

		if(nodeRefArray.length == 0) {
			println("error: way is empty");
			return;
		}

		w.nodeArray = new PVector[nodeRefArray.length];

		for (int k = 0; k < nodeRefArray.length; k++) {         
  			XML nd = nodeRefArray[k];
  			String ndRef = nd.getString("ref");

  			PVector v = nodeMap.get(ndRef);
  			if(v == null) {
  				println("error: " + ndRef + " node is not present");
  				return;
  			}

  			w.nodeArray[k] = v;
		}
```

Draw map
--------

The drawing in processing is just a formality. We first find the scaling factor.

```
	<<scale-factor>> =
		float scaleFactorX = (float)width / t.maxX;
    	float scaleFactorY = (float)height / t.maxY;
    	scaleFactor = min(scaleFactorX, scaleFactorY);
```

Then we draw simply the ways.

```
	<<draw operation>> =
		stroke(0);

	    for(int i = 0; i < t.wayArray.length; i++) {
	    	Way w = t.wayArray[i];
	    	PVector lastPoint = null;

	    	for(int j = 0; j < w.nodeArray.length; j++) {
	    		PVector point = w.nodeArray[j];
	    		if(lastPoint != null) {
	    			line(lastPoint.x * scaleFactor, - lastPoint.y * scaleFactor + height,
	    				 point.x * scaleFactor, - point.y * scaleFactor + height);
	    		}
	    		lastPoint = point;
	    	}
	    }
```

Conclusion
----------

What we see, is that the `.osm` format is pretty simple but the LoadXML method in processing is not accurate for this problem. File are too large and we must use a parser that don't load all the file in memory first.

You can find the source code in [https://github.com/chamot1111/blog_programination]()

Appendix
========

```
	<<./OpenStreetMap/OpenStreetMap.pde>> =
		<<::OpenStreetMap.pde>>
```

```
	<<./OpenStreetMap/Helper.pde>> =
		<<::Helper.pde>>
```

```
	<<./OpenStreetMap/GraphTree.pde>> =
		<<::GraphTree.pde>>
```
