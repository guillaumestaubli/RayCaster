using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;

namespace RayCaster.Utils
{
    static class MathExt
    {
        private static double _rad_15 = DegToRad(15);

        #region RADS

        public static double RAD_0 { get { return DegToRad(0); } }

        public static double RAD_15 { get { return DegToRad(15); } }
        public static double RAD_30 { get { return DegToRad(30); } }
        public static double RAD_45 { get { return DegToRad(45); } }
        public static double RAD_60 { get { return DegToRad(60); } }
        public static double RAD_75 { get { return DegToRad(75); } }

        public static double RAD_90 { get { return DegToRad(90); } }

        public static double RAD_105 { get { return DegToRad(105); } }
        public static double RAD_120 { get { return DegToRad(120); } }
        public static double RAD_135 { get { return DegToRad(135); } }
        public static double RAD_150 { get { return DegToRad(150); } }
        public static double RAD_165 { get { return DegToRad(165); } }

        public static double RAD_180 { get { return DegToRad(180); } }

        public static double RAD_195 { get { return DegToRad(195); } }
        public static double RAD_210 { get { return DegToRad(210); } }
        public static double RAD_225 { get { return DegToRad(225); } }
        public static double RAD_240 { get { return DegToRad(240); } }
        public static double RAD_255 { get { return DegToRad(255); } }

        public static double RAD_270 { get { return DegToRad(270); } }

        public static double RAD_285 { get { return DegToRad(285); } }
        public static double RAD_300 { get { return DegToRad(300); } }
        public static double RAD_315 { get { return DegToRad(315); } }
        public static double RAD_330 { get { return DegToRad(330); } }
        public static double RAD_345 { get { return DegToRad(345); } }

        public static double RAD_360 { get { return DegToRad(360.0); } }

        public static List<double> RADS = new List<double>();

        #endregion

        public static void Init()
        {
            for (int i = 0; i <= 360; i += 1)
            {
                RADS.Add(DegToRad(i));
            }
        }

        public static double DegToRad(double alpha)
        {
            return alpha * Math.PI / 180.0f;
        }

        public static double RadToDeg(double alpha)
        {
            return alpha * 180.0f / Math.PI;
        }

        public static int RoundCoordinates(double a)
        {
            double adjust = 0.5f;

            if ((int)a % 10 == 0)
                return (int)a;

            if ((int)(a + adjust) % 10 == 0)
                return (int)(a + adjust);

            if ((int)(a - adjust) % 10 == 0)
                return (int)(a - adjust);

            return (int)Math.Round(a);
        }

        public static Vector2d GetIntersection(Vector2d A1, Vector2d A2, Vector2d B1, Vector2d B2)
        {
            Vector2d A1A2 = A2 - A1;
            Vector2d B1B2 = B2 - B1;
            Vector2d A1B2 = B2 - A1;

            Vector2d crossing = A1 + Determinant(A1B2, B1B2) / Determinant(A1A2, B1B2) * A1A2;

            return crossing;
        }

        private static double Determinant(Vector2d A, Vector2d B)
        {
            return A.X * B.Y - A.Y * B.X;
        }

        public static double GetLength(Vector2d A, Vector2d B)
        {
            double delta_x = 0.0f;
            double delta_y = 0.0f;

            if (A.X > B.X)
                delta_x = A.X - B.X;
            else
                delta_x = B.X - A.X;

            if (A.Y > B.Y)
                delta_y = A.Y - B.Y;
            else
                delta_y = B.Y - A.Y;

            return Math.Sqrt(Math.Pow(delta_x, 2) + Math.Pow(delta_y, 2));
        }

        public static double CorrectRotationMultiples(double rad_angle)
        {
            if (rad_angle == 0.0)
                return rad_angle;

            if (rad_angle > 0)
            {
                while (rad_angle > RAD_360)
                    rad_angle -= RAD_360;
            }
            else
            {
                while (rad_angle < -1.0 * RAD_360)
                    rad_angle += RAD_360;
            }

            return rad_angle;
        }

        public static Vector2d RoundCoords(Vector2d coords)
        {
            double x = coords.X;
            double y = coords.Y;

            if (x % 10 == 0 || y % 10 == 0)
                return coords;

            double diff_x = x - Math.Floor(x);
            double diff_y = y - Math.Floor(y);

            double round_x = x;
            double round_y = y;

            if (diff_x < 0.001f)
            {
                round_x = Math.Floor(x);
            }
            else if (diff_x > 0.999f)
            {
                round_x = Math.Ceiling(x);
            }

            if (diff_y < 0.001f)
            {
                round_y = Math.Floor(y);
            }
            else if (diff_y > 0.999f)
            {
                round_y = (Math.Ceiling(y));
            }

            return new Vector2d(round_x, round_y);
        }

        public static double CorrectPerfectAngles(double alpha)
        {
            double a = Math.Round(alpha, 3);
            a = Math.Abs(a);

            foreach (double rad in MathExt.RADS)
            {
                if (a == Math.Round(rad, 3))
                {
                    if (alpha < 0)
                        return alpha -= 0.001;
                    else
                        return alpha += 0.001;
                }
            }

            return alpha;
        }
    }
}
