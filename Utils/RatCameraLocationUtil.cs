using System.IO;
using System.Collections.Generic;

namespace Datacom.Envirohack {

    public class CameraLocation {
        public string Camera {get;set;}
        public string GlobalId {get; set;}
        public double PointX {get; set;}
        public double PointY {get; set;}
    }

    // Static variable that loads camera_locations.csv into memory
    public static class CameraLocations {
        public static List<CameraLocation> Locations { get; set; }

        static CameraLocations() {
            Locations = new List<CameraLocation>();
            var csv = File.ReadAllLines("camera_locations.csv");

            for (int i = 1; i < csv.Length; i++) {
                var line = csv[i].Split(',');
                var location = new CameraLocation {
                    Camera = line[0],
                    GlobalId = line[1],
                    PointX = double.Parse(line[2]),
                    PointY = double.Parse(line[3])
                };
                Locations.Add(location);
            }
        }
    }
    public static class RatCameraLocationUtils {
        
        public static CameraLocation GetCameraLocation(string filename){
            var parts = filename.Split('_');
            var cameraName = parts[0];
            var location = CameraLocations.Locations.Find(x => x.Camera == cameraName);

            return location;
        }
    }
}