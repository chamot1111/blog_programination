
class Way {
    public int width;
    public int maxSpeed;
    public PVector[] nodeArray;
}

class GraphTree {
    public Way [] wayArray;

    public float maxX;
    public float maxY;

    float minlat;
    float minlon;
    float maxlat;
    float maxlon;

    GraphTree() {

    }

    public void LoadFile(String filename) {
        HashMap<String,PVector> nodeMap = new HashMap<String,PVector>();
        XML xml = loadXML(filename);
                
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
                
        XML[] nodes = xml.getChildren("node");
        for (int i = 0; i < nodes.length; i++) {
              XML n = nodes[i];
              float lat = n.getFloat("lat");
              float lon = n.getFloat("lon");
              String id = n.getString("id");
              PVector v = ConvertLatLongToMeters(lon, lat, minlon, minlat);
              nodeMap.put(id, v);
        }
                
        XML[] ways = xml.getChildren("way");
        wayArray = new Way[ways.length];
        for (int j = 0; j < ways.length; j++) {
              XML wNode = ways[j];
                
              Way w = new Way();
                
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
                
              wayArray[j] = w;
        }
    }
}