using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace VisualKeyboard
{
    class FontInfo{
        public string FontName;
        public float FontSize;
        public FontStyle _FontStyle;

        internal FontInfo(string _FontName, float _FontSize, FontStyle __FontStyle)
        {
            FontName = _FontName;
            FontSize = _FontSize;
            _FontStyle = __FontStyle;
        }
    };

    class ColorCord {
        public string key="";
        public PointF pointF;
        public int RX, RY;
        public bool Flag = false;

        internal ColorCord(string _key, float X, float Y, int _RX, int _RY, bool _Flag)
        {
            Flag = _Flag;
            key = _key;
            pointF.X = X;
            pointF.Y = Y;
            RX = _RX;
            RY = _RY;
        }


        internal void SetValFal()
        {
            Flag = false;
            key = "";
            pointF.X = -1;
            pointF.Y = -1;
            RX = -1;
            RY = -1;
        }

        public void SetVal(string _key, float X, float Y, int _RX, int _RY, bool _Flag)
        {
            Flag = _Flag;
            key = _key;
            pointF.X = X;
            pointF.Y = Y;
            RX = _RX;
            RY = _RY;
        }
    }

    class BoardKey
    {
        const int MaxLenthWord = 1024;
        const int keyNumver = 26;
        const int firline_KeyNumber = 10;
        const int Secline_KeyNumber = 9;
        const int Thrline_KeyNumber = 7;
        int[,] boundary = new int[4,2];
        int boundarybottom = 0;
        Graphics dc ;
        Font font ;
        SizeF sizeF; 
        Pen BluePen_Key;

        string[] KeyName = new string[] { 
                "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P",
                "A", "S", "D", "F", "G", "H", "J", "K", "L",
                "Z", "X", "C", "V", "B", "N", "M"};
        PointF[] KeyPosition = new PointF[keyNumver+1]; 


        internal BoardKey(int PositionLeft, int PositionUp, int KeySizeSqu, Graphics DC,FontInfo _FontInfo )
        {
            dc = DC;
            PointF pointF;
            BluePen_Key = new Pen(Color.Blue, 3);
            font = new Font(_FontInfo.FontName, _FontInfo.FontSize,_FontInfo._FontStyle);
            sizeF = dc.MeasureString(KeyName[0], font);

            //boundary calculation: left to right
            boundary[0,0] = PositionLeft;
            boundary[0,1] = PositionLeft+firline_KeyNumber*KeySizeSqu;
            boundary[1,0] = PositionLeft;
            boundary[1,1] = PositionLeft+Secline_KeyNumber*KeySizeSqu;
            boundary[2,0] = PositionLeft+KeySizeSqu;
            boundary[2,1] = PositionLeft+Thrline_KeyNumber*KeySizeSqu;
            boundary[3,0] = PositionLeft+KeySizeSqu;
            boundary[3,1] = PositionLeft+Thrline_KeyNumber*KeySizeSqu;
            boundarybottom = PositionUp + 4 * KeySizeSqu;

            int iPositionLeft = PositionLeft, iPositionUp = PositionUp;
            for (int i = 0; i < firline_KeyNumber; ++i)
            {
                pointF = new PointF(iPositionLeft + KeySizeSqu / 2 - sizeF.Width / 2,
                        iPositionUp + KeySizeSqu / 2 - sizeF.Height / 2 + 1);// Note: 1 is for correct the position
                KeyPosition[i].X = (float)iPositionLeft;
                KeyPosition[i].Y = (float)iPositionUp;
                dc.DrawRectangle(BluePen_Key, iPositionLeft, iPositionUp, KeySizeSqu, KeySizeSqu);
                dc.DrawString(KeyName[i], font, Brushes.Black, pointF);
                iPositionLeft += KeySizeSqu;
            }

            iPositionLeft = PositionLeft;
            iPositionUp += KeySizeSqu;
            for (int i = firline_KeyNumber; i < Secline_KeyNumber + firline_KeyNumber; ++i)
            {
                pointF = new PointF(iPositionLeft + KeySizeSqu / 2 - sizeF.Width / 2,
                     iPositionUp + KeySizeSqu / 2 - sizeF.Height / 2 + 1);// Note: 1 is for correct the position
                KeyPosition[i].X = (float)iPositionLeft;
                KeyPosition[i].Y = (float)iPositionUp;
                dc.DrawRectangle(BluePen_Key, iPositionLeft, iPositionUp, KeySizeSqu, KeySizeSqu);
                dc.DrawString(KeyName[i], font, Brushes.Black, pointF);
                iPositionLeft += KeySizeSqu;
            }

            iPositionLeft = PositionLeft+KeySizeSqu;
            iPositionUp += KeySizeSqu;
            for (int i = Secline_KeyNumber+firline_KeyNumber; i < keyNumver; ++i)
            {
                pointF = new PointF(iPositionLeft + KeySizeSqu / 2 - sizeF.Width / 2,
                     iPositionUp + KeySizeSqu / 2 - sizeF.Height / 2 + 1);// Note: 1 is for correct the position
                KeyPosition[i].X = (float)iPositionLeft;
                KeyPosition[i].Y = (float)iPositionUp;
                dc.DrawRectangle(BluePen_Key, iPositionLeft, iPositionUp, KeySizeSqu, KeySizeSqu);
                dc.DrawString(KeyName[i], font, Brushes.Black, pointF);
                iPositionLeft += KeySizeSqu;
            }

            iPositionLeft = PositionLeft + KeySizeSqu;
            iPositionUp += KeySizeSqu;
            dc.DrawRectangle(BluePen_Key, iPositionLeft, iPositionUp, KeySizeSqu * Thrline_KeyNumber, KeySizeSqu);
            KeyPosition[keyNumver].X = (float)iPositionLeft;
            KeyPosition[keyNumver].Y = (float)iPositionUp;
        }

        public ColorCord ConvCoord2String(int FormX, int FormY, int X, int Y, int KeySizeSqu, int SquMinDistance)
        {
            ColorCord Change0 = new ColorCord("",-1,-1,-1,-1,false);
            if (   ( (boundary[0, 0] <= X && X <= boundary[0, 1]) ||
                    (boundary[1, 0] <= X && X <= boundary[1, 1]) ||
                    (boundary[2, 0] <= X && X <= boundary[2, 1]) ||
                    (boundary[3, 0] <= X && X <= boundary[3, 1])) &&
                    (FormY <= Y && Y <= boundarybottom)
                )
            {//within the boundaries
                string s = "";
                int RX = (X - FormX) / KeySizeSqu, RY = (Y - FormY) / KeySizeSqu;
                int DeltaX = 0;
                int DeltaY = 0;
                    switch (RY)
                    {
                        case (0):
                            DeltaX = (int)(KeyPosition[RX].X) + KeySizeSqu/2 - X;
                            DeltaY = (int)(KeyPosition[RX].Y) + KeySizeSqu/2 - Y;
                            if ( DeltaX*DeltaX+DeltaY*DeltaY <= SquMinDistance ) 
                            {
                                Change0.SetVal(KeyName[RX], KeyPosition[RX].X, KeyPosition[RX].Y, RX, RY, true);
                            }
                            else
                            {
                                Change0.SetValFal();
                            }
                            break;
                        case (1):
                            if (RX < 9)
                            {
                                int index = RX + firline_KeyNumber;
                                DeltaX = (int)(KeyPosition[index].X) + KeySizeSqu / 2 - X;
                                DeltaY = (int)(KeyPosition[index].Y) + KeySizeSqu / 2 - Y;
                                if (DeltaX * DeltaX + DeltaY * DeltaY <= SquMinDistance)
                                {
                                    Change0.SetVal(KeyName[index], KeyPosition[index].X, KeyPosition[index].Y, RX, RY, true);
                                }
                                else
                                {
                                    Change0.SetValFal();
                                }
                            }
                            else
                            {
                                s = "";
                                Change0.SetValFal();
                            }
                            break;
                        case (2):
                            if (RX < 8 && RX > 0)
                            {
                                int index = RX + firline_KeyNumber + Secline_KeyNumber - 1;
                                DeltaX = (int)(KeyPosition[index].X) + KeySizeSqu/ 2 - X;
                                DeltaY = (int)(KeyPosition[index].Y) + KeySizeSqu/2 - Y;
                                if (DeltaX * DeltaX + DeltaY * DeltaY <= SquMinDistance)
                                {
                                    Change0.SetVal(KeyName[index], KeyPosition[index].X, KeyPosition[index].Y, RX, RY, true);
                                }
                                else
                                {
                                    Change0.SetValFal();
                                }
                            }
                            else
                            {
                                s = "";
                                Change0.SetValFal();
                            }
                            break;
                        case (3):
                            if (0 < RX && RX < 8)
                            {
                                s = " ";
                                Change0.SetVal(" ", KeyPosition[keyNumver].X, KeyPosition[keyNumver].Y, RX, RY, true);
                            }
                            else
                            {
                                s = "";
                                Change0.SetValFal();
                            }
                            break;
                    }
            }
            else
            {
                Change0.SetValFal();
            }
            return Change0;
        }

        public void KeyIndexPlus(ColorCord pos, FontInfo KeyFont, int KeySizeSqu, int times)
        {
            PointF pointF = new PointF(pos.pointF.X + KeySizeSqu - sizeF.Width,
                        pos.pointF.Y );
            dc.DrawString(times.ToString(), font, Brushes.Red, pointF);
        }
 
        public string PrintPos()
        {
            string s = System.String.Empty;
            for(int i=0;i<keyNumver+1;i++)
            {
                s += string.Format("<{0},{1}>\n", KeyPosition[i].X, KeyPosition[i].Y);
            }
            return s;
        }

    };

}
