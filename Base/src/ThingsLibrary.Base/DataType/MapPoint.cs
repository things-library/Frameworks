using System.Diagnostics.CodeAnalysis;

namespace ThingsLibrary.DataType
{    
    public class MapPoint : IEquatable<MapPoint>, IComparable<MapPoint>
    {
        [JsonPropertyName("x")]
        public double X { get; init; }

        [JsonPropertyName("y")]
        public double Y { get; init; }

        [JsonPropertyName("z")]
        public double Z { get; init; }

        [JsonPropertyName("srid"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int SRID { get; init; }

        [JsonIgnore]
        public (double, double) Coordinate => (this.X, this.Y);
                
        public MapPoint() { }

        public MapPoint(double x, double y, int srid)
        {
            this.X = x;
            this.Y = y;

            this.SRID = srid;
        }

        public MapPoint(double x, double y, double z, int srid)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;

            this.SRID = srid;
        }       

        public MapPoint((double, double) coordinate, int srid)
        {
            this.X = coordinate.Item1;
            this.Y = coordinate.Item2;
            
            this.SRID = srid;
        }

        #region --- IEquatable Methods ---

        public static bool operator ==(MapPoint entity1, MapPoint entity2)
        {
            if (Object.ReferenceEquals(entity1, null) && System.Object.ReferenceEquals(entity2, null)) { return true; }

            if (Object.ReferenceEquals(entity1, null)) { return false; }
            if (Object.ReferenceEquals(entity2, null)) { return false; }

            return (
                entity1.X == entity2.X &&
                entity1.Y == entity2.Y &&
                entity1.Z == entity2.Z &&
                entity1.SRID == entity2.SRID);
        }

        public static bool operator !=(MapPoint entity1, MapPoint entity2)
        {
            if (Object.ReferenceEquals(entity1, null) && System.Object.ReferenceEquals(entity2, null)) { return false; }

            if (Object.ReferenceEquals(entity1, null)) { return true; }
            if (Object.ReferenceEquals(entity2, null)) { return true; }

            return !(
                entity1.X == entity2.X &&
                entity1.Y == entity2.Y &&
                entity1.Z == entity2.Z &&
                entity1.SRID == entity2.SRID);
        }

        public bool Equals(MapPoint? compareEntity)
        {
            if (compareEntity is null) { return false; }

            return (
                this.X == compareEntity.X &&
                this.Y == compareEntity.Y &&
                this.Z == compareEntity.Z &&
                this.SRID == compareEntity.SRID);
        }

        public override bool Equals(object? obj)
        {
            if (Object.ReferenceEquals(obj, null)) { return false; }
            if (!(obj is MapPoint)) { return false; }

            return Equals((MapPoint)obj);
        }
        
        #endregion

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(MapPoint? other)
        {     
            if(other is null) { return -1; }

            if(other.X == this.X)
            {
                return this.Y.CompareTo(other.Y);
            }
            else
            {
                return this.X.CompareTo(other.X);
            }

        }

        #region --- Static Methods ---

        public MapPoint Get((double, double) coordinate, int srid)
        {
            return new MapPoint(coordinate, srid);
        }

        public static double BearingToLocation(MapPoint point1, MapPoint point2)
        {
            double latitude1 = DegreesToRadians(point1.Y);
            double latitude2 = DegreesToRadians(point2.Y);

            double longitudeDifference = DegreesToRadians(point2.X - point1.X);

            double y = System.Math.Sin(longitudeDifference) * System.Math.Cos(latitude2);
            double x = System.Math.Cos(latitude1) * System.Math.Sin(latitude2) - System.Math.Sin(latitude1) * System.Math.Cos(latitude2) * System.Math.Cos(longitudeDifference);

            return (RadiansToDegrees(System.Math.Atan2(y, x)) + 360) % 360;
        }

        private static MapPoint LocationToRad(MapPoint pointA)
        {
            return new MapPoint(DegreesToRadians(pointA.X), DegreesToRadians(pointA.Y), pointA.Z, pointA.SRID);
        }

        //public RoutePoint GetParallelPoint(MapPoint mapPoint, double course, double distance, bool isLeft)
        //{
        //    MapPoint endPoint = FindPointAtCourse(mapPoint, 100, course);

        //    double[] point1 = new double[2];

        //    point1[0] = this.Longitude;
        //    point1[1] = this.Latitude;

        //    DotSpatial.Projections.Reproject.ReprojectPoints(point1, new double[1] { 0 }, DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984, ProjectionTools.GetUtmZoneWgs84(this.Latitude, this.Longitude), 0, 1);

        //    RoutePoint startPointUtm = new RoutePoint(point1[0], point1[1]);

        //    double[] point2 = new double[2];

        //    point2[0] = endPoint.Longitude;
        //    point2[1] = endPoint.Latitude;

        //    DotSpatial.Projections.Reproject.ReprojectPoints(point2, new double[1] { 0 }, DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984, ProjectionTools.GetUtmZoneWgs84(this.Latitude, this.Longitude), 0, 1);

        //    MapPoint endPointUtm = new RoutePoint(point2[0], point2[1]);


        //    double theta = System.Math.Atan2(startPointUtm.Longitude - endPointUtm.Longitude, startPointUtm.Latitude - endPointUtm.Latitude) + (System.Math.PI / 2);
        //    double dl = System.Math.Sqrt(((startPointUtm.Longitude - endPointUtm.Longitude) * (startPointUtm.Longitude - endPointUtm.Longitude)) + ((startPointUtm.Latitude - endPointUtm.Latitude) * (startPointUtm.Latitude - endPointUtm.Latitude)));

        //    if (theta > System.Math.PI)
        //    {
        //        theta -= System.Math.PI * 2;
        //    }

        //    double dx = System.Math.Round(distance * System.Math.Sin(theta));
        //    double dy = System.Math.Round(distance * System.Math.Cos(theta));

        //    double[] parallelPoint = new double[2];

        //    if (isLeft)
        //    {
        //        parallelPoint = new double[2] { startPointUtm.Longitude + dx, startPointUtm.Latitude + dy };
        //    }
        //    else
        //    {
        //        parallelPoint = new double[2] { startPointUtm.Longitude - dx, startPointUtm.Latitude - dy };
        //    }

        //    DotSpatial.Projections.Reproject.ReprojectPoints(parallelPoint, new double[1] { 0 }, ProjectionTools.GetUtmZoneWgs84(this.Latitude, this.Longitude), DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984, 0, 1);

        //    RoutePoint result = new RoutePoint(parallelPoint[0], parallelPoint[1]);
        //    result.Course = course;
        //    return result;
        //}

        /// <summary>
        /// Returns a point that is at the specified distance and course of the start point
        /// </summary>
        /// <param name="mapPoint">Starting Point</param>
        /// <param name="distance">Distance in meters</param>
        /// <param name="course">Degrees</param>
        /// <returns><see cref="MapPoint"/></returns>
        public static MapPoint FindPointAtCourse(MapPoint mapPoint, double distance, double course)
        {
            double bearing = DegreesToRadians(course);

            return FindPointAtBearing(mapPoint, distance, bearing);
        }

        /// <summary>
        /// Returns a point that is at the specified distance and bearing of the start point
        /// </summary>
        /// <param name="startPoint">Starting Point</param>
        /// <param name="distance">Distance</param>
        /// <param name="bearing">Bearing (Degrees)</param>
        /// <returns><see cref="MapPoint"/></returns>
        public static MapPoint FindPointAtBearing(MapPoint startPoint, double distance, double bearing)
        {
            double initialBearingRadians = bearing;

            const double radiusEarthMetres = 6371010;

            double distRatio = distance / radiusEarthMetres;
            double distRatioSine = System.Math.Sin(distRatio);
            double distRatioCosine = System.Math.Cos(distRatio);

            double startRadX = DegreesToRadians(startPoint.X);
            double startRadY = DegreesToRadians(startPoint.Y);


            double startLatCos = System.Math.Cos(startRadY);
            double startLatSin = System.Math.Sin(startRadY);

            double endRadsY = System.Math.Asin((startLatSin * distRatioCosine) + (startLatCos * distRatioSine * System.Math.Cos(initialBearingRadians)));
            double endRadsX = startRadX + System.Math.Atan2(System.Math.Sin(initialBearingRadians) * distRatioSine * startLatCos, distRatioCosine - startLatSin * System.Math.Sin(endRadsY));

            return new MapPoint
            {
                Y = RadiansToDegrees(endRadsY),
                X = RadiansToDegrees(endRadsX),
                SRID = startPoint.SRID
            };
        }


        public static double DegreesToRadians(double degrees)
        {
            const double degToRadFactor = System.Math.PI / 180;
            return degrees * degToRadFactor;
        }

        public static double RadiansToDegrees(double radians)
        {
            const double radToDegFactor = 180 / System.Math.PI;
            return radians * radToDegFactor;
        }

        public int GetHashCode([DisallowNull] MapPoint obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
