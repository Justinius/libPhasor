using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libPhasor
{
    public class phasor
    {
        private double mX_cord;
        private double mY_cord;
        private double mRadius;
        private double mAngle;
        private int error_num;
        private int number_dec_places;
        private double mult;
        public bool isError;

        #region utility functions
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }
        #endregion

        #region private phasor update helper functions
        private void update_on_radius()
        {
            //when radius is changed need to update rest of the stuff
            //since the angle hasn't changed need to change the x,y coordinate
            //it should be collinear with the current x,y
            if (mX_cord == 0)
            {
                mY_cord = mRadius;
                return;
            }
            if (mY_cord == 0)
            {
                mX_cord = mRadius;
                return;
            }

            double slope = mY_cord / mX_cord;
            //y = slope*x
            //mRadius = sqrt(pow(slope*x,2)+pow(x,2))
            //mRadius = sqrt((slope^2)*x^2 + x^2)
            //mRadius = sqrt((1+slope^2) * x^2)
            //mRadius = x*sqrt(1+slope^2)
            //if mX_cord < 0 then it should remain negative
            //x = mRadius/sqrt(1+slope^2)
            //y = slope*x

            double temp_x = mRadius / Math.Sqrt(1 + Math.Pow(slope, 2));
            if (mX_cord < 0 && temp_x > 0) //need to change sign
                temp_x = -1 * temp_x;

            mX_cord = temp_x;
            mY_cord = slope * mX_cord;
        }
         
        private void update_on_angle()
        {
            //update the phasor based on angle
            //this updates the x and the y as well
            //x = r * cos(theta)
            //y = r * sin(theta)
            double ang = deg2rad(mAngle);
            mX_cord = mRadius * Math.Cos(ang);
            mY_cord = mRadius * Math.Sin(ang);
        }
         
        private void update_on_cord()
        {

            #region check for NaN and Infinity
            if (Double.IsNaN(mX_cord) || Double.IsNaN(mY_cord))
            {
                mX_cord = 0;
                mY_cord = 0;
                mRadius = 0;
                mAngle = 0;
                error_num = 3;
                isError = true;
            }

            if (Double.IsInfinity(mX_cord) || Double.IsInfinity(mY_cord))
            {
                mX_cord = 0;
                mY_cord = 0;
                mRadius = 0;
                mAngle = 0;
                error_num = 4;
                isError = true;
            }
            #endregion

            
            //when updating the x cord alone
            //have to update the radius and the angle
            mRadius = Math.Sqrt(Math.Pow(mX_cord, 2) + Math.Pow(mY_cord, 2));
            mAngle = rad2deg(Math.Atan2(mY_cord, mX_cord));
        }
        #endregion

        #region public accessors
        public double mag
        {
            get { return mRadius; }
            set { 
                mRadius = value;
                update_on_radius(); //update phasor based on changed radius
            }
        }

        public double angle_deg
        {
            get { return mAngle; }
            set { 
                mAngle = value;
                update_on_angle(); //update phasor based on changed angle
            }
        }

        public double angle_rad
        {
            get { return deg2rad(mAngle); }
            set
            {
                mAngle = rad2deg(value);
                update_on_angle(); //update phasor based on changed angle
            }
        }

        public double x
        {
            get { return mX_cord; }
            set { 
                mX_cord = value;
                update_on_cord(); //update phasor based on changed x position
            }
        }

        public double y
        {
            get { return mY_cord; }
            set { 
                mY_cord = value;
                update_on_cord(); //update phasor based on changed y position
            }
        }
        
        public int num_places
        {
            get { return number_dec_places; }
            set
            {
                number_dec_places = value;
                if (number_dec_places > 10)
                    number_dec_places = 10;
                else if (number_dec_places < 0)
                    number_dec_places = 0;

                mult = Math.Pow(10, number_dec_places);
            }
        }
        #endregion

        #region operator overloading
        public static phasor operator +(phasor p1, phasor p2)
        {
            return new phasor(p1.x + p2.x, p1.y + p2.y, Math.Min(p1.num_places, p2.num_places), true);
        }

        public static phasor operator -(phasor p1, phasor p2)
        {
            return new phasor(p1.x - p2.x, p1.y - p2.y, Math.Min(p1.num_places, p2.num_places), true);
        }

        public static phasor operator *(phasor p1, phasor p2)
        {
            return new phasor(p1.mag * p2.mag, p1.angle_deg + p2.angle_deg, Math.Min(p1.num_places, p2.num_places), false);
        }

        public static phasor operator *(double c, phasor p2)
        {
            return new phasor(c*p2.mag, p2.angle_deg, p2.num_places, false);
        }

        public static phasor operator *(phasor p1, double c)
        {
            return new phasor(c * p1.mag, p1.angle_deg, p1.num_places, false);
        }

        public static phasor operator /(phasor p1, phasor p2)
        {
            return new phasor(p1.mag/p2.mag, p1.angle_deg - p2.angle_deg, Math.Min(p1.num_places, p2.num_places), false);
        }
             
        public static phasor operator /(phasor p1, double c)
        {
            return new phasor(p1.mag / c, p1.angle_deg, p1.num_places, false);
        }

        public static phasor operator /(double c, phasor p1)
        {
            //dividing by a scalar by a phasor
            //we treat as a complex number, multiply top and bottom by complex conjugate
            //get p1.x and its y and complex conjugate
            double x_val = p1.x;
            double y_val = p1.y;
            double y_conj_val = -1 * y_val;
            
            //denominator becomes x^2 + y^2
            //and we have the original scalar c
            double mult_term = c / (x_val * x_val + y_val * y_val);
            //numerator is just the complex conjugate

            return ((new phasor(x_val, y_conj_val, p1.num_places, true))*mult_term);
        }

        public static bool operator ==(phasor p1, phasor p2)
        {
            if (p1.x == p2.x && p1.y == p2.y)
                return true;
            else
                return false;
        }

        public static bool operator !=(phasor p1, phasor p2)
        {
            if (p1.x != p2.x || p1.y != p2.y)
                return true;
            else
                return false;
        }

        public override bool Equals(object o)
        {
            try
            {
                return (bool)(this == (phasor)o);
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (int)Math.Floor(mX_cord * mY_cord);
        }

        #endregion

        public void phas2rect(ref double x_out, ref double y_out)
        {
            double ang = deg2rad(mAngle);
            x_out = mRadius * Math.Cos(ang);
            y_out = mRadius * Math.Sin(ang);
        }

        public void comp_conj()
        {
            this.mY_cord = -1 * this.mY_cord;
            update_on_cord();
        }

        public void comp_conj(phasor p)
        {
            this.mX_cord = p.x;
            this.mY_cord = -1*p.y;
            update_on_cord();
        }

        public string error_str()
        {
            string error_string = "No Error.";
            switch (error_num)
            {
                case 1:
                    error_string = "Constructor called with NaN value.";
                    break;
                case 2:
                    error_string = "Constructor called with Infinity value.";
                    break;
                case 3:
                    error_string = "Update coordinate called with NaN value.";
                    break;
                case 4:
                    error_string = "Update coordinate called with Infinity value.";
                    break;
            }

            return error_string;
        }
        
        public override string ToString()
        {
            return String.Format("{0} /_ {1}", Math.Truncate(mRadius*mult)/mult, Math.Truncate(mAngle*mult)/mult);
        }

        public phasor(double x_cord, double y_cord, bool rect)
        {

            #region check for NaN and Infinity
            if (Double.IsNaN(x_cord) || Double.IsNaN(y_cord))
            {
                this.mX_cord = 0;
                this.mY_cord = 0;
                this.mRadius = 0;
                this.mAngle = 0;
                this.error_num = 1;
                this.isError = true;
                this.number_dec_places = 3;
                this.mult = 1000.0;
            }

            if (Double.IsInfinity(x_cord) || Double.IsInfinity(y_cord))
            {
                this.mX_cord = 0;
                this.mY_cord = 0;
                this.mRadius = 0;
                this.mAngle = 0;
                this.error_num = 2;
                this.isError = true;
                this.number_dec_places = 3;
                this.mult = 1000.0;
            }

            if (isError)
                return;
            #endregion
            
            if (rect) //rectangular coordinates
            {
                this.mX_cord = x_cord;
                this.mY_cord = y_cord;

                //radius is distance from origin to the point
                this.mRadius = Math.Sqrt(Math.Pow(x_cord, 2) + Math.Pow(y_cord, 2));
                this.mAngle = rad2deg(Math.Atan2(y_cord, x_cord));
            }
            else //treat them as r, theta
            {
                this.mRadius = x_cord;
                this.mAngle = y_cord;
                double ang = deg2rad(mAngle);
                this.mX_cord = x_cord * Math.Cos(ang);
                this.mY_cord = x_cord * Math.Sin(ang);
            }
            this.number_dec_places = 3;
            this.mult = Math.Pow(10, this.number_dec_places);
        }

        public phasor(double x_cord, double y_cord, int num_display, bool rect)
        {

            #region check for NaN and Infinity
            if (Double.IsNaN(x_cord) || Double.IsNaN(y_cord))
            {
                this.mX_cord = 0;
                this.mY_cord = 0;
                this.mRadius = 0;
                this.mAngle = 0;
                this.error_num = 1;
                this.isError = true;
                this.number_dec_places = 3;
                this.mult = 1000.0;
            }

            if (Double.IsInfinity(x_cord) || Double.IsInfinity(y_cord))
            {
                this.mX_cord = 0;
                this.mY_cord = 0;
                this.mRadius = 0;
                this.mAngle = 0;
                this.error_num = 2;
                this.isError = true;
                this.number_dec_places = 3;
                this.mult = 1000.0;
            }

            if (isError)
                return;
            #endregion

            if (rect) //rectangular coordinates
            {
                this.mX_cord = x_cord;
                this.mY_cord = y_cord;

                //radius is distance from origin to the point
                this.mRadius = Math.Sqrt(Math.Pow(x_cord, 2) + Math.Pow(y_cord, 2));
                this.mAngle = rad2deg(Math.Atan2(y_cord, x_cord));
            }
            else //treat them as r, theta
            {
                this.mRadius = x_cord;
                this.mAngle = y_cord;
                double ang = deg2rad(mAngle);
                this.mX_cord = x_cord * Math.Cos(ang);
                this.mY_cord = x_cord * Math.Sin(ang);
            }
            this.number_dec_places = num_display;
            this.mult = Math.Pow(10, this.number_dec_places);
        }

        public phasor(phasor p)
        {
            this.mX_cord = p.x;
            this.mY_cord = p.y;
            this.mRadius = p.mag;
            this.mAngle = p.angle_deg;
            this.number_dec_places = p.num_places;
            this.mult = Math.Pow(10, this.number_dec_places);
        }

        public phasor()
        {
            this.mX_cord = 0;
            this.mY_cord = 0;
            this.mRadius = 0;
            this.mAngle = 0;
            this.number_dec_places = 3;
            this.mult = Math.Pow(10, this.number_dec_places);
        }
    }
}
