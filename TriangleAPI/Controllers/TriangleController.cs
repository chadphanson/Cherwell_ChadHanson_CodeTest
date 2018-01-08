using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Drawing;

namespace TriangleAPI.Controllers
{
    [RoutePrefix("api/triangles")]

    public class TriangleController : ApiController
    {
        public Dictionary<string, int> RowNumbers =
            new Dictionary<string, int>();

        
        [Route("getcoordinatesbyrowandcolumn/{row}/{column}")]
        public HttpResponseMessage GetCoordinatesByRowAndColumn(string Row, int Column)
        {
            List<Point> returnPoints = new List<Point>();
            bool columnIsEven = Column % 2 == 0;

            // load row/letter mapping
            LoadDictionary();            

            // check for valid input value
            if (!RowNumbers.Any(c => c.Key == Row) || (Column < 1 || Column > 12))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "That is not a valid row/column input.");
            }

            int RowNumber = RowNumbers.Single(c => c.Key == Row).Value;

            // *** FIRST COORDINATE ***
            int Coord1X = 0;
            // get X pos
            // -- if column is even, multiply it by 10 then reduce by half. 
            // -- else, multiple it by 10, subtract 10, then divide in half
            Coord1X = columnIsEven ? ((Column * 10) / 2) : (((Column * 10) - 10) / 2);

            // get Y pos. if column is even, add 10
            int Coord1Y = RowNumber * -10;
            if (columnIsEven) { Coord1Y += 10; }     

            Point Coord1 = new Point(Coord1X, Coord1Y);
            returnPoints.Add(Coord1);

            // *** SECOND COORDINATE ***
            // get X pos
            // -- if column is even, subtract 10 from Coord1X, else use Coord1X
            int Coord2X = columnIsEven ? Coord2X = Coord1X - 10 : Coord2X = Coord1X;
            // get Y pos
            // -- if column is even, use Coord1Y, else add 10 to Coord1Y
            int Coord2Y = columnIsEven ? Coord2Y = Coord1Y : Coord2Y = Coord1Y + 10;          

            Point Coord2 = new Point(Coord2X, Coord2Y);
            returnPoints.Add(Coord2);

            // *** THIRD COORDINATE ***
            // get X pos
            // -- if column is even, use Coord1X, else add 10 to Coord1X
            int Coord3X = columnIsEven ? Coord3X = Coord1X : Coord3X = Coord1X + 10;
            // get Y pos
            // -- if column is even, use Coord1Y - 10, else use Coord1Y
            int Coord3Y = columnIsEven ? Coord3Y = Coord1Y - 10 : Coord3Y = Coord1Y;

            Point Coord3 = new Point(Coord3X, Coord3Y);
            returnPoints.Add(Coord3);

            return Request.CreateResponse(HttpStatusCode.OK, returnPoints);
        }

        [Route("getrowandcolumnbycoordinates")]
        [HttpPost]
        public HttpResponseMessage GetRowAndColumnByCoordinates([FromBody] List<Point> coords)
        {
            // load row/letter mapping
            LoadDictionary();

            Point Coord1 = coords[0];
            Point Coord2 = coords[1];
            Point Coord3 = coords[2];

            // determine if triangle is oriented up (odd column) or down (even column) based on given coords
            bool columnIsEven = Coord2.X == Coord1.X - 10 ? true : false;

            // calculate to find Row and Column values
            int Row = columnIsEven ? Math.Abs(Coord1.Y / 10) + 1 : Math.Abs(Coord1.Y / 10);
            int Column = columnIsEven ? (Coord1.X / 5) : (((Coord1.X / 10) * 2) + 1);

            // check for values outside of acceptable ranges
            if (!RowNumbers.Any(c => c.Value == Row) || (Column < 1 || Column > 12))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No triangle found for those coordinates.");
            }
            
            string RowStr = RowNumbers.Single(c => c.Value == Row).Key;
            string ColumnStr = Column.ToString();

            return Request.CreateResponse(HttpStatusCode.OK, RowStr + ColumnStr);            
        }

        public void LoadDictionary()
        {
            RowNumbers.Add("A", 1);
            RowNumbers.Add("B", 2);
            RowNumbers.Add("C", 3);
            RowNumbers.Add("D", 4);
            RowNumbers.Add("E", 5);
            RowNumbers.Add("F", 6);
        }
    }
}