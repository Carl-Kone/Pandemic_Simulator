using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using static CovidMeetsHogwarts.PandemicSimulator;

namespace CovidMeetsHogwarts
{
    class Program
    {
        public static string graphFilename = "graph.dot";
        public static int numberOfHumans = 100;
        public static int days = 60;
        public static bool setRandomValues = true; // if set to false,
        // you can manually set the global hygiene, social_distancing, travelling_rate in Human.cs

        public static int generatedImageWidth = 380;
        public static int generatedImageHeight = 590;
        
        static void Main(string[] args)
        { 
            
            // generate graph from file
            Graph graph = Graph.FromFile("../../../../tests/" + graphFilename);
            if (graph == null)
            {
                Console.Error.WriteLine("Failed to create graph: wrong format");
                return;
            }
  
            // create location object with this graph
            Location hogwarts = new Location(graph, numberOfHumans, setRandomValues);
  
            // generate image of initial state
            GenerateImage(hogwarts, 0);
  
            // infect a random human with a virus
            InitializePandemic(hogwarts);
            GenerateImage(hogwarts, 1);

            // simulate pandemic
            int infectiousCount = 1;
            int i = 2;
            for (; i < days && infectiousCount != 0; i++) // stops if there is no infectious humans
            {
                // update and generate image
                infectiousCount =  UpdatePandemic(hogwarts);
                GenerateImage(hogwarts, i);
            }
            
            // generate GIF of all generated images
            GenerateGIF(i);
        }

        /// <summary>
        /// generate image that shows how the pandemic looks like at the given location
        /// at update 'i'. The image will be created from a file written in a specific
        /// graph description language (DOT) through a program (dot) that can read and render
        /// the latter in a graphical form.
        /// </summary>
        /// <param name="location">The location where the pandemic is studied</param>
        /// <param name="day">The number of days (untis of time) that have passed
        /// since the initialization of the pandemic</param>
        public static void GenerateImage(Location location, int day)
        {
            // create DOT file and write location's info in the right format
            string filename = "pandemic-update-" + day + ".dot";
            StreamWriter writer = new StreamWriter(filename);
            writer.Write(location);
            
            // close DOT file
            writer.Close();
            
            // create a process to convert dot file to an image
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            
            // select the right executable depending on the OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C dot ";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                startInfo.FileName = "/usr/local/bin/dot";
            }
            else
            {
                startInfo.FileName = "/usr/bin/dot";
            }

            double width = Math.Round((double) generatedImageWidth / 100, 2);
            double height = Math.Round((double) generatedImageHeight / 100, 2);
            
            startInfo.Arguments += "-Tpng -Gsize=" + width + "," + height + "\\! -Gdpi=100 " + 
                                   filename + " -o pandemic-update-" + day + ".png";
            process.StartInfo = startInfo;
            
            // run process and wait for it to finish
            process.Start();
            process.WaitForExit();
            
            Console.WriteLine("Image {0} complete!", day);
        }

        /// <summary>
        /// generate a GIF showing the pandemic's progression within given 'numberOfImages' days / units of time.
        /// The GIF will be created from the images of each day of the pandemic thanks to
        /// the program ImageMagick.
        /// </summary>
        /// <param name="numberOfImages">The number of images generated in this simulation</param>
        public static void GenerateGIF(int numberOfImages)
        {
            Console.WriteLine("Converting the {0} images into GIF..................", numberOfImages);
            
            // get all the images generated in the current simulation
            string images = "";
            for (int i = 0; i < numberOfImages; i++)
            {
                images += "pandemic-update-" + i + ".png ";
            }
            
            // create a process to convert images to a GIF
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            
            // select the right executable depending on the OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C magick ";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                startInfo.FileName = "/usr/local/bin/magick";
            }
            else
            {
                startInfo.FileName = "/usr/bin/magick";
            }
            
            startInfo.Arguments += "convert -gravity center " +
                                  "-background white -extent " + generatedImageWidth + 
                                  "x" + generatedImageHeight + " -delay 20 " + 
                                  images + "-loop 1 ../../../../tests/pandemic.gif";
            process.StartInfo = startInfo;
            
            // run process and wait for it to finish
            process.Start();
            process.WaitForExit();
            
            Console.WriteLine("Conversion completed!");
        }
    }
}