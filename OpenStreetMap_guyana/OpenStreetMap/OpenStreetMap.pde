
GraphTree t;
float scaleFactor;

void setup()
{
    size(800, 800);

    // Test graphtree with a guyane map
    t = new GraphTree();
    t.LoadFile("../guyane-latest.osm");

    float scaleFactorX = (float)width / t.maxX;
    float scaleFactorY = (float)height / t.maxY;
    scaleFactor = min(scaleFactorX, scaleFactorY);

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

void draw() {
    
}

