using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment4COEN320
{
    /**
     * Needed:
     * Complete -1) A random number generator. Use the pseudo code on page 443 or 444 of the textbook by R.Jain. 
     * Complete -2) A routine to generate exponentially distributed random numbers.
     * Complete -3) A routine to compute the waiting time in the queue (use the formula)
     * Complete -4) A routine to collect data.
     * Complete -5) A routine to reduce data
     * Complete -6) A routine to copmute the theoretical result of the M/M/1 queue on page 16 of the Simulation paper.
     * Complete -7) Main part of the program.
     * 
     * Customer Arrival Stream Seed: 727,633,698
     * Customer Service Completion Seed: 1,335,826,707
     **/


    
    class Program
    {
        //--------------Variables---------------
        //Measurement Number
        int m = 10000;

        //Waiting Time
        double totalWaitingTime;
        List<double> wList = new List<double>(); //Waiting Time List

        //Interarrival Time------
        int aExpectedValue =200; //The expected value of the interarrival times
        int arrivalSeed = 727633698;
        double totalArrivalTime = 0;
        List<double> aList = new List<double>();

        //Service Time------
        int sExpectedValue = 100; //The expected value of the service times
        int serviceSeed = 1335826707;
        double totalServiceTime = 0;
        List<double> sList = new List<double>();

        static void Main(string[] args)
        {
            Program program = new Program();
        }

        public Program()
        {
            Console.WriteLine("Beginning the Single Server Queue...\n");


            //Generating Customers
            for(int i = 0; i < m; i++)
            {
                int customerNumber = i;
                arrivalSeed = generateValue(arrivalSeed, aExpectedValue,ref totalArrivalTime, aList); //Generate value for arrival time of customer i
                serviceSeed = generateValue(serviceSeed, sExpectedValue,ref totalServiceTime, sList); //Generate value for service time of customer i
                waitingTime(customerNumber); //Calculate waiting time for customer i
            }

            //Results
            Console.WriteLine("---Simulation Results:");
            Console.WriteLine("Arrival Seed: " + arrivalSeed);
            Console.WriteLine("Arrival Expected Value: " + aExpectedValue);
            Console.WriteLine("Service Seed: " + serviceSeed);
            Console.WriteLine("Service Expected Value: " + sExpectedValue);
            Console.WriteLine();

            Console.WriteLine("m: " + m);
            Console.WriteLine("Average s: " + totalServiceTime/m);
            Console.WriteLine("Average a: " + totalArrivalTime/m);
            Console.WriteLine("Average w: " + totalWaitingTime/m);
            Console.WriteLine("Utilization: " + (totalServiceTime/m)/(totalArrivalTime/m));

            Console.WriteLine();
            Console.WriteLine("------Theoretical Results:");
            Console.WriteLine(getTheoreticalResultsToString());

            Console.ReadLine();
        }

        /*Generates one value given its seed and expected value and return that new value of the seed. Adds value to the list that was referenced.
         */ 
        public int generateValue(int seed, int expectedValue,ref double averageRef, List<double> gList)
        {
            int w = randomNumberGenerator(seed);
            double zeroToOne = w / 2147483647.0;
            double value = (generateExponentiallyRandom(zeroToOne, expectedValue));
            averageRef += value;
            gList.Add(value);
            return w; //Return the new seed number
        }

        /**Random number generator using interger arithmetic based on figure 26.2 of Jain. 
        **/
        public int randomNumberGenerator(int x) //x is the seed
        {
            //Const
            const int a = 16807; //Multiplier
            const int m = 2147483647; //Modulus
            const int q = 127773; //m div a
            const int r = 2836; // m mod a

            //Variables
            int x_div_q, x_mod_q, x_new;
            x_div_q = x / q;
            x_mod_q = x % q;
            x_new = a * x_mod_q - r * x_div_q;
            if (x_new > 0){
                x = x_new;
            }
            else{
                x = x_new + m;
            }
            return x;
        }

        /**Convert the 0 to 1 Uniform distribution to an exponetially distributed random number
         **/
        public double generateExponentiallyRandom(double x, double expectedValue)
        {
            double lambda = 1.0 /expectedValue;
            x = -(1.0 / lambda) * Math.Log(1.0 - x); //From http://www.ece.virginia.edu/mv/edu/prob/stat/random-number-generation.pdf
            return x;
        }

        /** Calculate the waitingLine for a specific customer
         **/
        public void waitingTime(int customerNumber)
        {
            if (customerNumber == 0){
                wList.Add(0);
            }
            else{
                double wTime = (wList[customerNumber - 1] + sList[customerNumber - 1] - aList[customerNumber - 1]);
                if (wTime < 0){
                    wList.Add(0);
                }
                else{
                    wList.Add(wTime);
                    totalWaitingTime += wTime; //Adds to the total waiting time of all customers
                }
            }
        }

        public string getTheoreticalResultsToString()
        {
            double lambda = 1f / aExpectedValue;
            double mu = 1f / sExpectedValue;
            double rho = lambda/mu;
            double wq = rho / (mu*(1f - rho));
            double w = wq + (1f / mu);
            double lq = lambda * wq;
            double l = lambda * w;

            String result =
                   "Lambda: " + lambda + "\n" +
                   "Mu: " + mu + "\n" +
                   "Utilization: Rho =  " + rho + "\n" +
                   "Mean queueing time: Wq = " + wq + "\n" +
                   "Mean residence time: W = " + w + "\n" +
                   "Mean number in queue: Lq = " + lq + "\n" +
                   "Mean Number in system: L = " + l + "\n";

            return result;

        }
    }
}