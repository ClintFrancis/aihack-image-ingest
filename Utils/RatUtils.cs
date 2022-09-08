using System.IO;
using System.Collections.Generic;

namespace Datacom.Envirohack {

    // Static variable that loads camera_locations.csv into memory
    public static class CameraLocations {
        public static List<Location> Locations { get; set; }

        static CameraLocations() {
            Locations = new List<Location>();
            var csv = File.ReadAllLines("camera_locations.csv");

            for (int i = 1; i < csv.Length; i++) {
                var line = csv[i].Split(',');
                var location = new Location {
                    Name = line[0],
                    GlobalId = line[1],
                    PointX = double.Parse(line[2]),
                    PointY = double.Parse(line[3]),
                    Type = "ArcGIS"
                };
                Locations.Add(location);
            }
        }
    }
    public static class RatUtils {
        
        public static Location GetCameraLocation(string filename){
            var parts = filename.Split('_');
            var cameraName = parts[0];
            var location = CameraLocations.Locations.Find(x => x.Name == cameraName);

            return location;
        }

        public static Fauna CreateRat(string filename) {
            var rat = new Fauna ();
            rat.CommonName = "Norwegian Rat";
            rat.ScientificName = "Rattus norvegicus";
            rat.SpeciesOrder = "Rodentia";
            rat.IsInvasive = true;
            rat.IsNative = false;
            rat.IsThreatened = false;
            rat.IsProtected = false;
            rat.IsPredator = true;
            rat.Tags = new List<Tag>{
                // TODO: Add tags
            };
            rat.Metadata = new MetaData{
                Source = filename,
                ProcessType = "Cognitive Services Computer Vision",
                Tags = new string[] {"Rat", "Detection", "Waiheke Island"}
            };
            rat.Location = GetCameraLocation(filename);

            return rat;
        }
    }
}