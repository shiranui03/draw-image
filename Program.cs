﻿using System;
using System.Drawing;
using System.Linq;

namespace draw_image
{
    class Program
        {
        // base color used for creating complex colors, value of base colors are in hex decimal 64-bit
        static int[] cColors = { 0x000000, 0x000080, 0x008000, 0x008080, 0x800000, 0x800080, 0x808000, 0xC0C0C0, 0x808080, 0x0000FF, 0x00FF00, 0x00FFFF, 0xFF0000, 0xFF00FF, 0xFFFF00, 0xFFFFFF };

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Bitmap image1 = new Bitmap(@"images\shinomiya.jpg", true);
            ConsoleWriteImage(image1); // execute drawing of image
            Console.ReadLine();
        }

        // create pixel with specific color using the base colors
        // it will try to get the nearest equal color from the base color as describe in the given image
        // used the concept of Color distance RGB as base algorithm
        public static void ConsoleWritePixel(Color cValue)
        {
            Color[] cTable = cColors.Select(x => Color.FromArgb(x)).ToArray();
            char[] rList = new char[] { (char)9617, (char)9618, (char)9619, (char)9608 }; // 1/4, 2/4, 3/4, 4/4
            int[] bestHit = new int[] { 0, 0, 4, int.MaxValue }; //ForeColor, BackColor, Symbol, Score
            try {
                for (int rChar = rList.Length; rChar > 0; rChar--)
                {
                    for (int cFore = 0; cFore < cTable.Length; cFore++)
                    {
                        for (int cBack = 0; cBack < cTable.Length; cBack++)
                        {
                            int R = (cTable[cFore].R * rChar + cTable[cBack].R * (rList.Length - rChar)) / rList.Length;
                            int G = (cTable[cFore].G * rChar + cTable[cBack].G * (rList.Length - rChar)) / rList.Length;
                            int B = (cTable[cFore].B * rChar + cTable[cBack].B * (rList.Length - rChar)) / rList.Length;
                            int iScore = (cValue.R - R) * (cValue.R - R) + (cValue.G - G) * (cValue.G - G) + (cValue.B - B) * (cValue.B - B);
                            if (!(rChar > 1 && rChar < 4 && iScore > 50000)) // rule out too weird combinations
                            {
                                if (iScore < bestHit[3])
                                {
                                    bestHit[3] = iScore; //Score
                                    bestHit[0] = cFore;  //ForeColor
                                    bestHit[1] = cBack;  //BackColor
                                    bestHit[2] = rChar;  //Symbol
                                }
                            }
                        }
                    }
                }
                Console.ForegroundColor = (ConsoleColor)bestHit[0];
                Console.BackgroundColor = (ConsoleColor)bestHit[1];
                Console.Write(rList[bestHit[2] - 1]);

            } catch (Exception e) {
                Console.Write("x");
                Console.WriteLine("Error In Pixel Creation: " + e.Message);
            }
            
        }

        // will write the image in the console
        // size of the image will be reduce to fit in the cmd (reduction by ratio)
        public static void ConsoleWriteImage(Bitmap source)
        {
            int sMax = 39;
            decimal percent = Math.Min(decimal.Divide(sMax, source.Width), decimal.Divide(sMax, source.Height));
            Size dSize = new Size((int)(source.Width * percent), (int)(source.Height * percent));   
            Bitmap bmpMax = new Bitmap(source, dSize.Width * 2, dSize.Height);
            try {
                for (int i = 0; i < dSize.Height; i++)
                {
                    for (int j = 0; j < dSize.Width; j++)
                    {
                        ConsoleWritePixel(bmpMax.GetPixel(j * 2, i));
                        ConsoleWritePixel(bmpMax.GetPixel(j * 2 + 1, i));
                    }
                    System.Console.WriteLine();
                }
                Console.ResetColor();
            } catch (Exception e) {
                Console.Write("x");
                Console.WriteLine("Error In Writing Image: " + e.Message);
            }
            
        }

        // algorithm trial two for color distance (failure)
        public static int ToConsoleColor(System.Drawing.Color c)
        {
            int index = (c.R > 128 | c.G > 128 | c.B > 128) ? 8 : 0;
            index |= (c.R > 64) ? 4 : 0;
            index |= (c.G > 64) ? 2 : 0;
            index |= (c.B > 64) ? 1 : 0;
            return index;
        }

        // algorithm trial two for writing the image (failure)
        public static void ConsoleWriteImageTwo(Bitmap src)
        {
            int min = 39;
            decimal pct = Math.Min(decimal.Divide(min, src.Width), decimal.Divide(min, src.Height));
            Size res = new Size((int)(src.Width * pct), (int)(src.Height * pct));
            Bitmap bmpMin = new Bitmap(src, res);
            for (int i = 0; i < res.Height; i++)
            {
                for (int j = 0; j < res.Width; j++)
                {
                    Console.ForegroundColor = (ConsoleColor)ToConsoleColor(bmpMin.GetPixel(j, i));
                    Console.Write("██");
                }
                System.Console.WriteLine();
            }
        }

        // algorithm trial three for writing the image (failure)
        // also merge it with the algorithm for color distance
        public static void ConsoleWriteImageThree(Bitmap bmpSrc)
        {
            int sMax = 39;
            decimal percent = Math.Min(decimal.Divide(sMax, bmpSrc.Width), decimal.Divide(sMax, bmpSrc.Height));
            Size resSize = new Size((int)(bmpSrc.Width * percent), (int)(bmpSrc.Height * percent));
            Func<System.Drawing.Color, int> ToConsoleColor = c =>
            {
                int index = (c.R > 128 | c.G > 128 | c.B > 128) ? 8 : 0;
                index |= (c.R > 64) ? 4 : 0;
                index |= (c.G > 64) ? 2 : 0;
                index |= (c.B > 64) ? 1 : 0;
                return index;
            };
            Bitmap bmpMin = new Bitmap(bmpSrc, resSize.Width, resSize.Height);
            Bitmap bmpMax = new Bitmap(bmpSrc, resSize.Width * 2, resSize.Height * 2);
            for (int i = 0; i < resSize.Height; i++)
            {
                for (int j = 0; j < resSize.Width; j++)
                {
                    Console.ForegroundColor = (ConsoleColor)ToConsoleColor(bmpMin.GetPixel(j, i));
                    Console.Write("██");
                }

                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("    ");

                for (int j = 0; j < resSize.Width; j++)
                {
                    Console.ForegroundColor = (ConsoleColor)ToConsoleColor(bmpMax.GetPixel(j * 2, i * 2));
                    Console.BackgroundColor = (ConsoleColor)ToConsoleColor(bmpMax.GetPixel(j * 2, i * 2 + 1));
                    Console.Write("▀");

                    Console.ForegroundColor = (ConsoleColor)ToConsoleColor(bmpMax.GetPixel(j * 2 + 1, i * 2));
                    Console.BackgroundColor = (ConsoleColor)ToConsoleColor(bmpMax.GetPixel(j * 2 + 1, i * 2 + 1));
                    Console.Write("▀");
                }
                System.Console.WriteLine();
            }
        }


        
    }
}