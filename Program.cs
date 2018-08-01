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
     * Complete - 1) A random number generator. Use the pseudo code on page 443 or 444 of the textbook by R.Jain. 
     * Complete - 2) A routine to generate exponentially distributed random numbers.
     * 3) A routine to compute the waiting time in the queue (use the formula)
     * 4) A routine to collect data.
     * 5) A routine to reduce data
     * 6) A routine to copmute the teoretical result of the M/M/1 queue on page 16 of the Simulation paper.
     * 7) Main part of the program.
     * 
     * Customer Arrival Stream Seed: 727,633,698
     * Customer Service Completion Seed: 1,335,826,707
     **/


    
    class Program
    {
        //--------------Variables---------------
        //Others
        int m = 100000;//The number of measurements
        double utilization = 0;

        //Waiting Time
        double waitingAverage;
        List<double> wList = new List<double>(); //Waiting Time List

        //Interarrival Time------
        int aExpectedValue =200; //The expected value of the interarrival times
        int arrivalSeed = 727633698;
        double arrivalAverage = 0;
        List<double> aList = new List<double>();

        //Service Time------
        int sExpectedValue = 100; //The expected value of the service times
        int serviceSeed = 1335826707;
        double serviceAverage = 0;
        List<double> sList = new List<double>();

        //Theoretical results


        static void Main(string[] args)
        {
            Program program = new Program();
        }

        public Program()
        {
            Console.WriteLine("Beginning the Single Server Queue...");

            //-----------------Interarrival Times Generation-------------------------
            {
                Console.WriteLine("--------Interarrival");
                generateValues(arrivalSeed, aExpectedValue, ref arrivalAverage, aList);
                Console.WriteLine("Avg:"+arrivalAverage);
               
            }
            //-----------------Service Times Generation-------------------------
            {
                Console.WriteLine("--------Service Time");
                generateValues(serviceSeed, sExpectedValue, ref serviceAverage, sList);
                Console.WriteLine("Avg:"+serviceAverage);
            }
            //-----------------Waiting Lines--------------------------------------
            {
                Console.WriteLine("--------Waiting Time");
                waitingLine(ref waitingAverage);
                Console.WriteLine("Avg:"+waitingAverage);
            }
            //-----------------Other---------------------------------------------------
            utilization = serviceAverage / arrivalAverage;
            Console.WriteLine("Utilization: " + utilization);
            Console.WriteLine(getTheoreticalResultsToString());

            Console.ReadLine();
        }

        public void generateValues(int seed, int expectedValue,ref double average, List<double> gList)
        {
            int w = seed;

            for (int i = 0; i < m; i++)
            {
                w = random(w); //Generates a new number given this seed;
                double zeroToOne = w / 2147483647.0;
                double value = (exponentiallyRandom(zeroToOne, expectedValue));
                average += value;
                //Console.WriteLine(value);
                gList.Add(value);
            }
            average = average / m;
        }

        /**Random number generator using interger arithmetic based on figure 26.2 of Jain. 
        **/
        public int random(int x) //x is the seed
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
            if (x_new > 0)
            {
                x = x_new;
            }
            else
            {
                x = x_new + m;
            }
            return x;
        }

        /**Convert the 0 to 1 Uniform distribution to an exponetially distributed random number
         * 
         **/
        public double exponentiallyRandom(double x, double expectedValue)
        {
            double lambda = 1.0 /expectedValue;
            x = -(1.0 / lambda) * Math.Log(1.0 - x); //From http://www.ece.virginia.edu/mv/edu/prob/stat/random-number-generation.pdf
            return x;
        }

        /**Calculates the waiting line between the Arrival and Service time
         **/
        public void waitingLine(ref double waitingAverage)
        {
            for(int n = 0; n < m; n++)
            {
                if(n == 0)
                {
                    wList.Add(0);
                    //Console.WriteLine(0);
                }
                else
                {
                    double wTime = (wList[n - 1] + sList[n - 1] - aList[n - 1]);
                    if( wTime < 0)
                    {
                        wList.Add(0);
                    }
                    else
                    {
                        wList.Add(wTime);
                        waitingAverage = waitingAverage + wTime;
                    }
                    //Console.WriteLine(wTime);
                }
            }
            waitingAverage = waitingAverage / m;
        }

        public string getTheoreticalResultsToString()
        {
            double lambda = 1f / aExpectedValue;
            double mu = 1f / sExpectedValue;
            double rho = lambda/mu;
            double wq = rho / (mu*(1f - rho));
            double w = wq + 1f / mu;
            double lq = lambda * wq;
            double l = lambda * w;

            String result = "---Theoretical Results: \n" +
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
