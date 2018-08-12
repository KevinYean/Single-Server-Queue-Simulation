using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment4COEN320 {
    /**
     * Customer Arrival Stream Seed: 727,633,698
     * Customer Service Completion Seed: 1,335,826,707
     **/

    class DiscreteEventSimulationQueue {
        static void Main(string[] args) {
            Console.WriteLine("Beginning the Single Server Queue...\n");
            DiscreteEventSimulationQueue program = new DiscreteEventSimulationQueue();
        }

        //--------------Parameters---------------//
        int arrivalSeed = 727633698; int serviceSeed = 1335826707; //Seeds
        int te = 1000000; int aExpectedValue = 200; int sExpectedValue = 180; //Simulations Variables

        List<double> sList = new List<double>(); //Service Time List, each length is the service time not TSi
        List<double> aList = new List<double>(); //Arrival Time List, each is intearrival time not Tai
        List<double> tAiList = new List<double>(); //Arrival 
        List<double> tSiList = new List<double>(); //Service
        double totalTSi = 0; double totalServiceTime = 0; double totalArrivalTime = 0;

        public DiscreteEventSimulationQueue() {
            int n = 0; //Number of customers currently in the system.
            double completionNumber = 0;
            double t1 = 0f;
            double t2 = te;
            double currentTime = 0f;


            //Generating Customers
            while (currentTime < te) {
                //Console.WriteLine("N = " + n);
                if (t1 < t2) { //Arrival
                    currentTime = t1;
                    t1 = currentTime + generateValue(ref arrivalSeed, aExpectedValue, ref totalArrivalTime, currentTime, aList); //Generate value for service time of customer i
                    tAiList.Add(t1);
                    n++;

                    if (n == 1) { //Meaning only this one is in the system
                        currentTime = t1;
                        double tempServiceTime = generateValue(ref serviceSeed, sExpectedValue, ref totalTSi, currentTime, sList);
                        t2 = currentTime + tempServiceTime;
                        tSiList.Add(t2);
                        totalServiceTime += tempServiceTime;
                    }
                }
                else { //Completion
                    currentTime = t2;
                    n--;
                    completionNumber++;//Completion
                    if (n > 0) {
                        double tempServiceTime = generateValue(ref serviceSeed, sExpectedValue, ref totalTSi, currentTime, sList);
                        if (tAiList[(int)completionNumber] > t2) { //If the arrival time is greater than the service than generate a service time from the arrival time
                            t2 = tAiList[(int)completionNumber] + tempServiceTime;
                        }
                        else { //If the arrival time is less than the service generate a service time from the previous service time
                            t2 = currentTime + tempServiceTime;
                        }
                        tSiList.Add(t2);
                        totalServiceTime += tempServiceTime;
                    }
                    else {
                        t2 = te;
                    }
                }
            }

            Console.WriteLine("------Simulation Results:");
            Console.WriteLine(getSimulationResultsToString(completionNumber));
            Console.WriteLine("------Theoretical Results:");
            Console.WriteLine(getTheoreticalResultsToString());

            Console.ReadLine();
        }

        /*Generates one value given its seed and expected value and return that new value of the seed. Adds value to the list that was referenced.
         */
        public double generateValue(ref int seed, int expectedValue, ref double averageRef, double val, List<double> gList) {
            seed = randomNumberGenerator(seed);
            double zeroToOne = seed / 2147483647.0;
            double value = (generateExponentiallyRandom(zeroToOne, expectedValue));
            averageRef += (value + val);
            gList.Add(value); //Add to list to keep track
            return value; //Return the new seed number
        }


        public string getTheoreticalResultsToString() {
            double t = te; //Period of time over which the system is measured.

            double a = t / aExpectedValue; // Number of arrivals in time T. Assuming a steady flow at the expected arrival rate.
            double lambda = a / t; // Arrival rate
            //If period T is sufficiently long, the number of arrivals observed should be approximately equal to to the number of completion which in that 
            //case means lamda = X; Assumption is called the flow balance assumption.
            double x = lambda;

            double b = sExpectedValue * a; // Total busy time which should be higher than the time period of plateau at it.
            double u = b / te; //Server utilization

            double ts = b / a; //Mean service time per customer;
            double mu = 1f / sExpectedValue;
            double wq = u / (mu * (1f - u)); //Mean queuing time;
            double w = wq + ts;

            double lq = lambda * wq;
            double l = lq + u;

            String result =
                   "B:" + b + "\n" +
                   "\n" +
                   "Throughput: X = " + x + "\n" + //Throughput x is lambda if T period is sufficiently long enough. Flow of balance assumption
                   "Utilization: U = " + u + "\n" +
                   "Mean Number in system: L = " + l + "\n" +
                   "Mean residence time: W = " + w + "\n";

            return result;

        }


        public string getSimulationResultsToString(double completionNumber) {
            double a = aList.Count; // Number of arrivals in time T. 
            double arrivalRate = a / te;
            //Calculate B
            double b = 0; // Total busy time which should be higher than the time period of plateau at it.
            if (completionNumber > 0) {
                for (int i = 1; i < completionNumber; i++) {
                    b += sList[i];
                }
            }
            double u = b / te;

            //Calculate W
            double residenceTime = 0;
            for (int i = 0; i < completionNumber; i++) {
                residenceTime += (tSiList[i] - tAiList[i]);
            }
            double w = residenceTime / completionNumber;

            //Calculate L
            double l = residenceTime / te;

            string toString = "Arrival Expected Value: " + aExpectedValue + "\n" +
                "Service Expected Value: " + sExpectedValue + "\n" +
                "Te: " + te + "\n" +
                "Total Customers Arrival: " + aList.Count() + "\n" +
                "Total Customer Completion: " + completionNumber +
                "\n" +
                "B:" + b + "\n" +
                "\n" +
                "X= " + completionNumber / te + "\n" +
                "U= " + b / te + "\n" +
                "L= " + l + "\n" +
                "W= " + w + "\n";

            return toString;
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
            if (x_new > 0) {
                x = x_new;
            }
            else {
                x = x_new + m;
            }
            return x;
        }

        /**Convert the 0 to 1 Uniform distribution to an exponetially distributed random number
         **/
        public double generateExponentiallyRandom(double x, double expectedValue) {
            double lambda = 1.0 / expectedValue;
            x = -(1.0 / lambda) * Math.Log(1.0 - x); //From http://www.ece.virginia.edu/mv/edu/prob/stat/random-number-generation.pdf
            return x;
        }
    }
}