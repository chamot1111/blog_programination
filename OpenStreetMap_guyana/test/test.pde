// increase memory allocated in preference

GraphTree t;
float scaleFactor;


void DrawLayer() {
	println("draw");
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
}

void setup()
{
    size(1024, 1024);
    // try to convert distance between Clermont-Ferrand and Paris
    // Clermont ferrand: {lon: 45.77667 lat: 3.08243}
    // Paris: {lon: 48.85615 lat: 2.35266}
    PVector clermontToParis = ConvertLatLongToMeters(48.85615 , 2.35266, 45.77667, 3.08243);
    println("Distance Clermont -> Paris: {x: " + clermontToParis.x + " m, y: " + clermontToParis.y + " m}");

    // Test graphtree with test.xml
    t = new GraphTree();
    t.LoadFile("../guyane-latest.osm");

    float scaleFactorX = (float)width / t.maxX;
    float scaleFactorY = (float)height / t.maxY;
    scaleFactor = min(scaleFactorX, scaleFactorY);

    println("scaleFactor: " + scaleFactor);

    DrawLayer();
}

void draw() {

}

