//////////////////////////////////////////////////////////////////////////////////////
//
//  Project: Project 1 - Traveling Salesman Problem
//  File Name: TravellingSalesmanDriver.cs
//  Description: Finds thee optimal route of a traveling salesman through a given set of coordinates
//  Course: CSCI 3230-001 - Algorithms
//  Author: Seth Norton, nortonsp@etsu.edu
//  Created: Saturday, January 25, 2020
//  Copyright: Seth Norton, 2020
//
//////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The name space which holds the methods to calculate the traveling salesman problem
/// </summary>
namespace SethNorton_ProjectOne_TravellingSalesman
{
   

    /// <summary>
    /// The class which holds the methods to calculate the traveling salesman problem
    /// </summary>
    class TravellingSalesmanDriver
    {
        /// <summary>
        /// The driver which calls other methods to figure out the optimal distance for the traveling salesman to travel
        /// </summary>
        /// <param name="args">The command line arguments of the program</param>
        static void Main(string[] args)
        {

            Console.WriteLine("How many points? ");
             
            int numberOfPoints = Int32.Parse(Console.ReadLine());                           //the number of points we are calculating the shortest route for
            int[] xs = new int[numberOfPoints];                                             //The x array which stores the x coordinates
            int[] ys = new int[numberOfPoints];                                             //The y array which stores the x coordinates
            int[] citiesOrder = new int[numberOfPoints];                                    //The order of the cities
            int[] optimalPath = new int[numberOfPoints];                                    //The optimal path
            double[,] citiesDistances = new double[numberOfPoints, numberOfPoints];         //stores an x and y coordinate
            int[] original = new int[numberOfPoints];                                       //the original path
            for (int i = 0; i < numberOfPoints; i++)
            {
                string instring = Console.ReadLine();                                       //gets the points and puts them into a string
                string[] temp = instring.Split(' ');                                        //splits the points up and then assigns the first to the x coord and the second to the y coord in the subsequent lines
                xs[i] = Int32.Parse(temp[0]);
                ys[i] = Int32.Parse(temp[1]);
                citiesOrder[i] = i;
                original[i] = i;

            }//end  for (int i = 0; i < numberOfPoints; i++)
            Stopwatch sw = Stopwatch.StartNew();    //Starts a new stopwatch
            original.Reverse();                     //reverses the array to use for checking 


            CalculateDistancesLookup(xs, ys, ref citiesDistances);    //distances work correctly 
            double totalDistance = GeneratePermutations(ref citiesOrder, ref citiesDistances, ref optimalPath, xs, ys, ref original);   //gets the total distance traveled

            sw.Stop();                                                //Stops the stopwatch
            Console.WriteLine($"Total Seconds: {sw.Elapsed.TotalMilliseconds / 1000}");
            Console.WriteLine($"Total Distance: {totalDistance}");
            Console.Write($"Optimal Route: ");
           PrintArray(optimalPath);

            Console.ReadLine();
        }

        /// <summary>
        /// Generates permutations which are used for finding the shortest path
        /// </summary>
        /// <param name="citiesOrder">the order of the cities in which we are testing the distance</param>
        /// <param name="distanceTable">pre calculated values to easily grab the distance for each permutation</param>
        /// <param name="optimalPath">holds the optimal path for the TSP problem</param>
        /// <param name="xs">The x coordinates stored in an array</param>
        /// <param name="ys">The y coordinates stored in an array</param>
        /// <param name="original">The original order of the route</param>
        /// <returns>The shortest routes distance</returns>
        public static double GeneratePermutations(ref int[] citiesOrder, ref double[,] distanceTable, ref int[] optimalPath, int[] xs, int[] ys, ref int[] original)
        {
            bool notFinished = true;        //determines if we are done generating permutations
            double totalDistance = 0.0;     //the total distabce of a specific path
            double shortestDistance = 0.0; //the shortest distance of a path
            


            //Forms the first route and makes it the best route
            totalDistance = FormRoute(ref citiesOrder, distanceTable, xs, ys, null);
            shortestDistance = totalDistance;   //first route is always the shortest
            Array.Copy(citiesOrder, optimalPath, citiesOrder.Length);

            while (notFinished == true)
            {
                notFinished = NextPermutation(ref citiesOrder); //catch whether or not we have generated all the permutations or not


                if (citiesOrder[0] < citiesOrder[citiesOrder.Length - 1])           //if the first element is less than the last element then generate a new route
                {
                    totalDistance = FormRoute(ref citiesOrder, distanceTable, xs, ys, shortestDistance);
                    if (citiesOrder.Equals(original))                               //if the reverse of the array is what we started with then we are done
                        notFinished = false;

                }//end if (citiesOrder[0] < citiesOrder[citiesOrder.Length - 1])

                if (totalDistance < shortestDistance || shortestDistance == 0.0)    //get a new shortest distance if the distance is less than the previous shortest distance
                {
                    shortestDistance = totalDistance;
                    Array.Copy(citiesOrder, optimalPath, citiesOrder.Length);


                }//end if (totalDistance < shortestDistance || shortestDistance == 0.0)
                //Console.Write("\tShortest Path: ");
                //  PrintArray(optimalPath);
                //Console.Write($"   Shortest Distance: {shortestDistance}");
                // Console.WriteLine();

            } //end while (notFinished == true)


            return shortestDistance;

        }//end static double GeneratePermutations(ref int[] citiesOrder, ref double[,] distanceTable, ref int[] optimalPath, int[] xs, int[] ys, ref int[] original)



        
        /// <summary>
        /// Generates the next permutation based on a given array
        /// </summary>
        /// <param name="citiesOrder">the array to generate the next permutation based upon this array</param>
        /// <returns>Boolean to determine if we are done</returns>
        public static bool NextPermutation(ref int[] citiesOrder)
        {
            int i = citiesOrder.Length - 1; //start at the end of the array
            while (i > 0 && citiesOrder[i - 1] >= citiesOrder[i])
                i--;
            if (i <= 0)
                return false;   //we have reached the beginning of the array and can stop
            //Find element we need to swap with
            int j = citiesOrder.Length - 1;
            while (citiesOrder[j] <= citiesOrder[i - 1])
                j--;
            //Found the two elements we need to swap so perform the swap
            Swap(ref citiesOrder, i - 1, j);


            j = citiesOrder.Length - 1;
            //Perform a reverse
                 while (i < j)
                 {
                     Swap(ref citiesOrder, i, j );
                     i++;
                     j--;
                 }
          // Console.Write("Order: "); 
          //  PrintArray(citiesOrder);
            

            return true;

        }//end static bool NextPermuation(ref int[] citiesOrder)

        /// <summary>
        /// Calculates the lookup table to easily lookup distances
        /// </summary>
        /// <param name="citiesX">The x coordinate array of points</param>
        /// <param name="citiesY">The y coordinate array of points</param>
        /// <param name="distanceLookup">The distance table to lookup distances so we do not have to calculate multiple times</param>
        public static void CalculateDistancesLookup(int[] xs, int[] ys, ref double[,] distanceLookup)
        {
            for (int x = 0; x < xs.Length; x++)
            {
                for (int y = -1; y < ys.Length - 1; y++)
                {
                    if (distanceLookup[y + 1, x] != 0)
                    {
                        distanceLookup[x, y + 1] = distanceLookup[y + 1, x];
                    }//end if(distanceLookup[y+1, x] != 0)
                    else
                    {
                        distanceLookup[x, y + 1] = Math.Sqrt(Math.Pow(xs[x] - xs[y + 1], 2) + Math.Pow(ys[x] - ys[y + 1], 2));
                    }//end else


                }//end for (int y = -1; y < ys.Length - 1; y++)
            }//end  for (int x = 0; x < xs.Length; x++)

        }//end  static void CalculateDistancesLookup(int[] xs, int[] ys, ref double[,] distanceLookup)



        /// <summary>
        /// Forms a route based on a certain permutation
        /// </summary>
        /// <param name="citiesOrder">Makes a route based on a certain order</param>
        /// <param name="distanceTable">The table which we already calculated distances</param>
        /// <param name="xs">The x coordinate array of points</param>
        /// <param name="ys">The y coordinate array of points</param>
        /// <returns>The distance for the route</returns>
        public static double FormRoute(ref int[] citiesOrder, double[,] distanceTable, int[] xs, int[] ys, double? shortestDistance)
        {
            double distance = 0.0;  //The distance of a route
            int i;                  //counter variable and indexer for the distance table


            distance += Math.Sqrt(Math.Pow(0 - xs[citiesOrder[0]], 2) + Math.Pow(0 - ys[citiesOrder[0]], 2));       //add the origin to first point
            if (distance > shortestDistance)
                return Double.MaxValue;
            for (i = 0; i < distanceTable.GetLength(0) - 1; i++)
            {
               distance += distanceTable[citiesOrder[i], citiesOrder[i + 1]];   //add distances together
               if(distance > shortestDistance)
               {
                    return Double.MaxValue;
               }
            }//end for (i = 0; i < distanceTable.GetLength(0) - 1; i++)

            distance += Math.Sqrt(Math.Pow(0 - xs[citiesOrder[i]], 2) + Math.Pow(0 - ys[citiesOrder[i]], 2));       //add the origin to second point

            return distance;

        }//end static double FormRoute(ref int[] citiesOrder, double[,] distanceTable, int[] xs, int[] ys)


        /// <summary>
        /// Swaps two element of an array
        /// </summary>
        /// <param name="cities">The array to swap elements in</param>
        /// <param name="i">One index to swap</param>
        /// <param name="j">The other index to swap</param>
        private static void Swap(ref int[] cities, int i, int j)
        {
            var temp = cities[i];   //make a temp variable to store the value of cities[i]
            cities[i] = cities[j];
            cities[j] = temp;
        }//end static void Swap(ref int[] cities, int i, int j)


        /// <summary>
        /// Prints the array so we can see some paths
        /// </summary>
        /// <param name="cities">The order of the cities</param>
        private static void PrintArray(int[] cities)
        {
            for (int i = 0; i < cities.Length; i++)
            {   
                cities[i] += 1;                 //increment by one to make it easier to read
                Console.Write(cities[i] + " ");
                cities[i] -= 1;
            }//end for (int i = 0; i < cities.Length; i++)


        }//end static void PrintArray(int[] cities)

    }//end static void Main(string[] args)
}//end name space SethNorton_ProjectOne_TravellingSalesman
