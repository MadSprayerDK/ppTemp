using System;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using PpApi.Database;
using Location = PpApi.Models.Location;

namespace PpApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var locationRepository = new LocationRepository(new PpDbContext());
            var locations = locationRepository.GetLocationQueryable().Where(x => true).OrderBy(x => x.Name).ToList();

            return View(locations);
        }

        public ActionResult AddLocation(long timestamp, float x, float y, string name, bool waypoint = false)
        {
            var location = new Location
            {
                Name = name,
                Timestamp = timestamp,
                X = x,
                Y = y,
                Waypoint = waypoint
            };

            var locationRepository = new LocationRepository(new PpDbContext());
            locationRepository.CreateLocation(location);

            return Json(location.Id != 0, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadKml(string name)
        {
            var locationRepository = new LocationRepository(new PpDbContext());
            if(locationRepository.GetLocationQueryable().Count(x => x.Name == name) == 0)
                return Json("Can't find run with specified name", JsonRequestBehavior.AllowGet);

            var locations = locationRepository.GetLocationQueryable().Where(x => x.Name == name).ToList();

            HttpContext.Response.Clear();
            HttpContext.Response.ContentType = "application/vnd.google-earth.kml+xm";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + name + ".kml");

            XmlTextWriter kml = new XmlTextWriter(HttpContext.Response.OutputStream, Encoding.UTF8);

            kml.Formatting = Formatting.Indented;
            kml.Indentation = 3;

            kml.WriteStartDocument();

            kml.WriteStartElement("kml", "http://www.opengis.net/kml/2.2");
            kml.WriteStartElement("Document");

            for (int i = 0; i < locations.Count; i++)
            {
                long logicTime = 0;
                double distanceToPreviously = 0;

                var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
                var time = posixTime.AddMilliseconds(locations[i].Timestamp);

                if (i != 0)
                {
                    var difference = locations[i].Timestamp - locations[i - 1].Timestamp;
                    logicTime = difference;

                    var p1 = new GeoCoordinate(locations[i].X, locations[i].Y);
                    var p2 = new GeoCoordinate(locations[i - 1].X, locations[i - 1].Y);

                    distanceToPreviously = p1.GetDistanceTo(p2);
                }

                kml.WriteStartElement("Placemark");

                kml.WriteStartElement("Style");
                kml.WriteStartElement("IconStyle");
                kml.WriteStartElement("Icon");

                if (locations[i].Waypoint)
                    kml.WriteElementString("href", "http://www.google.com/intl/en_us/mapfiles/ms/icons/green-dot.png");
                else
                    kml.WriteElementString("href", "http://www.google.com/intl/en_us/mapfiles/ms/icons/red-dot.png");

                kml.WriteEndElement(); // <Icon>
                kml.WriteEndElement(); // <IconStyle>
                kml.WriteEndElement(); // <Style>

                kml.WriteElementString("name", "GPS Point");
                kml.WriteElementString("description", "Timestamp: " + time.ToString("dd-MM-yy - HH:mm:ss") + "<br />" +
                    "Logic Time: " + logicTime + " ms." + "<br />" +
                    "Distance to previous: " + distanceToPreviously + " m.");

                kml.WriteStartElement("Point");

                kml.WriteElementString("coordinates", locations[i].Y.ToString(CultureInfo.InvariantCulture) + "," + locations[i].X.ToString(CultureInfo.InvariantCulture) + ",0");

                kml.WriteEndElement(); // <Point>
                kml.WriteEndElement(); // <Placemark>
            }

            kml.WriteEndElement(); // <Document>
            kml.WriteEndDocument(); // <kml>
            kml.Flush();
            kml.Close();

            HttpContext.Response.End();

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}