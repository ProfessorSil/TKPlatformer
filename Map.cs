using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;
using System.IO;

namespace TKPlatformer
{
    class Map
    {
        public CollisionGrid colGrid;
        //TODO: Add scene grid values for making pretty scenes
        public Dictionary<string, string> properties;
        private string filePath;

        public float GridSize = 32;

        public int Width
        {
            get { return colGrid.Width; }
        }
        public int Height
        {
            get { return colGrid.Height; }
        }

        public Map(int width, int height)
        {
            colGrid = new CollisionGrid(width, height);

        }
        /// <summary>
        /// Loads all values from a .lvl file
        /// </summary>
        /// <param name="filename"> just filename, no folders, no filetype</param>
        /// <param name="baseFolder">the folder that we look in to find the file</param>
        /// <returns>whether it was successful</returns>
        public Map(string fileName, string baseFolder = "Content\\Levels\\")
        {
            LoadFromFile(fileName, baseFolder);
        }

        public RectangleF GetColRec(int x, int y)
        {
            return new RectangleF(x * GridSize, y * GridSize, GridSize, GridSize);
        }

        /// <summary>
        /// Loads all values from a .lvl file
        /// </summary>
        /// <param name="filename"> just filename, no folders, no filetype</param>
        /// <param name="baseFolder">the folder that we look in to find the file</param>
        /// <returns>whether it was successful</returns>
        public bool LoadFromFile(string filename, string baseFolder = "Content\\Levels\\")
        {
            filePath = baseFolder + filename + ".lvl";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Could not find file: " + filePath);
                return false;
            }

            Console.WriteLine("Loading level: '" + filePath + "'");
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line = reader.ReadLine();

                #region Parse Map Size
                int w, h;
                {
                    if (line == "" || !line.Contains(','))
                    {
                        Console.WriteLine("Could not find/read size of map");
                        return false;
                    }

                    int i = line.IndexOf(',');
                    if (!int.TryParse(line.Substring(0, i), out w))
                    {
                        Console.WriteLine("Could not parse width value");
                        return false;
                    }
                    if (!int.TryParse(line.Substring(i + 1, line.Length - i - 1), out h))
                    {
                        Console.WriteLine("Could not parse height value");
                        return false;
                    }
                }
                #endregion

                #region Load CollisionGrid Values
                {
                    colGrid = new CollisionGrid(w, h);

                    line = reader.ReadLine();

                    int lastIndex = 0;
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            int nextIndex = line.IndexOf('♦', lastIndex);
                            if (nextIndex == -1 || nextIndex == lastIndex)
                            {
                                //Either the line was "~" indicator or not long enough or no text inbetween two ♦ seperators
                                Console.WriteLine("Could not find specified type at " + x.ToString() + "," + y.ToString());
                                colGrid.SetValue(x, y, CollisionGrid.CollisionType.Empty);
                                if (nextIndex != -1)
                                    lastIndex = nextIndex + 1;
                            }
                            else
                            {
                                int val = -1;
                                if (!int.TryParse(line.Substring(lastIndex, nextIndex - lastIndex), out val))
                                {
                                    Console.WriteLine("Could not parse value at " + x.ToString() + "," + y.ToString() + ".");
                                    colGrid.SetValue(x, y, CollisionGrid.CollisionType.Empty);
                                }
                                else
                                {
                                    colGrid.SetValue(x, y, (CollisionGrid.CollisionType)val);
                                }

                                lastIndex = nextIndex + 1;
                            }
                        }

                        if (line == "~")
                        {
                            Console.WriteLine("Line " + y.ToString() + " of collision values does not exist. Expected to be " + h + " cells tall.");
                            //Do not read in next line yet.
                        }
                        else
                        {
                            line = reader.ReadLine();
                            lastIndex = 0;
                        }
                    }
                }
                #endregion

                #region Load Properties
                {
                    if (line != "~")
                    {
                        Console.WriteLine("Something went wrong. Expected '~' seperator line. Found: " + line);
                        Console.WriteLine("Regardless, other info loaded so returning successful load");
                        return true;
                    }

                    line = reader.ReadLine();

                    properties = new Dictionary<string, string>();
                    while (line != "" && line != null)
                    {
                        int i = line.IndexOf(':');
                        string id = line.Substring(0, i);
                        string value = line.Substring(i + 1, line.Length - i - 1);

                        if (id.ToLower() == "gridsize")
                        {
                            int size;
                            if (int.TryParse(value, out size))
                            {
                                GridSize = size;
                                line = reader.ReadLine();
                                continue;
                            }
                        }

                        properties.Add(id, value);

                        line = reader.ReadLine();
                    }
                }
                
                #endregion

                Console.WriteLine("Done Loading. Seemingly successful");
                return true;
            }
        }
    }
}
