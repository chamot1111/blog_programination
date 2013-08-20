static int kRadiusEarth_m = 6373000;

static public PVector ConvertLatLongToMeters(float lon, float lat, float lon_origin, float lat_origin) {
    PVector v = new PVector(0.0f, 0.0f);
    float dLat = lat - lat_origin;
    float dLon = lon - lon_origin;
    v.x = dLon * TWO_PI / 360.0f * kRadiusEarth_m;
    v.y = dLat * TWO_PI / 360.0f * kRadiusEarth_m;
    return v;
}
