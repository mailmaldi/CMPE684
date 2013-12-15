using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace SerialPortTest
{
    public class RssiValue
    {
        public int nodeid { get; set; }
        public float value { get; set; }
        public int values_count { get; set; } // for counting running average

        public RssiValue(int nodeid, float value, int values_count)
        {
            this.nodeid = nodeid;
            this.value = value;
            this.values_count = values_count;
        }

        public RssiValue(int nodeid, float value)
        {
            this.nodeid = nodeid;
            this.value = value;
            this.values_count = 1;
        }

        public string toString()
        {
            string str = "nodeid=" + this.nodeid + " value=" + this.value + " values_count=" + this.values_count;
            return str;
        }
    }
    /*End of class RssiValue */


    public class RssiValues
    {
        // key = value observed on node , value = list 
        public ConcurrentDictionary<int, List<RssiValue>> dictionary { get; set; }
        private int num_nodes = 5; // default 5 in the system, hardcoded since dhaval's functions are hardcoded

        public RssiValues(int num_nodes = 5)
        {
            this.dictionary = new ConcurrentDictionary<int, List<RssiValue>>();
            this.num_nodes = num_nodes;
        }


        public List<RssiValue> getValuesForNode(int nodeid)
        {
            List<RssiValue> value = null;

            if (dictionary.TryGetValue(nodeid, out value))
            {

                Console.WriteLine("Found For key = ", nodeid, " value= {", value.ToString(), "}");
                return value;
            }
            else
            {
                Console.WriteLine("NOT Found For key = ", nodeid);
                return null;
            }
        }

        // if average flag is true, then set the average values in dictionary, else just replace
        public void setValuesForNode(int nodeid, List<RssiValue> values, bool average)
        {
            if (average)
                setAverageValueToNode(nodeid, values);
            else
            {
                //dictionary.Add(nodeid, values);
                dictionary[nodeid] = values;
            }
        }

        private void setAverageValueToNode(int nodeid, List<RssiValue> values)
        {
            List<RssiValue> existing = getValuesForNode(nodeid);

            if (existing != null)
            {
                foreach (RssiValue item in values)
                {
                    bool found = false;
                    foreach (RssiValue existItem in existing)
                    {
                        if (item.nodeid == existItem.nodeid)
                        {
                            existItem.value = (existItem.value * existItem.values_count + item.value * item.values_count) / (existItem.values_count + item.values_count);
                            existItem.values_count += item.values_count;
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        existing.Add(item);
                    }
                }
            }
            else
            {
                setValuesForNode(nodeid, values, false);
            }
        }

        public string toString()
        {
            string str = "";
            foreach (var pair in dictionary)
            {
                str += "nodeid=" + pair.Key + " values={";
                if (pair.Value != null)
                {
                    foreach (RssiValue item in pair.Value)
                    {
                        str += "[" + item.toString() + "]";
                    }
                }
                str += "}\n";
            }
            return str;
        }

        // Assumes ALL NODES are in incremental order i.e. 0,1,2,3... and no skipping in between i.e. no 0,1,3,5,...
        // Assumes that in dictionary, the value will be a list of all indices loaded i.e. 0,1,2,3...
        public int[,] getRssiValuesMatrix()
        {
            int[,] returnMatrix = new int[5, 5];

            foreach (var pair in dictionary)
            {
                foreach (RssiValue item in pair.Value)
                {
                    returnMatrix[pair.Key, item.nodeid] = (pair.Key == item.nodeid) ? 1000 : (int)item.value;
                    //TODO DHAVAL TO USE THIS FOR HIS HARDCODED CLASS
                }
            }
            return returnMatrix;
        }

        public static void printMatrix(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    int s = matrix[i, j];
                    Console.Write(s + " ");
                }
                Console.WriteLine();
            }
        }

    }
    /*End of class RssiValues */


    public class TargetQueue
    {
        public int current_target { get; set; }
        public HashSet<int> targets { get; set; }

        public TargetQueue()
        {
            //initialize
            this.current_target = -1;
            this.targets = new HashSet<int>();
        }

        public void addTarget(int nodeid)
        {
            targets.Add(nodeid);
            if (current_target == -1)
                current_target = nodeid;
        }

        public void removeTarget(int nodeid)
        {
            targets.Remove(nodeid);
            if (this.current_target == nodeid)
                this.current_target = -1;
        }

        public int getTarget()
        {
            int returnVal = -1;

            if (targets.Count == 0)
                return returnVal;

            if (this.current_target != -1)
                return this.current_target;

            //TODO write a function that will iterate through all the elements of the HashSet and find the node that is closest to bot and return it
            foreach (int i in targets)
            {
                returnVal = i;
                break;

            }
            this.current_target = returnVal;
            return returnVal;

        }

        public string toString()
        {
            string str = "current_target=" + this.current_target + " targets={";
            foreach (int i in targets)
                str += i + " ";
            str += "}";
            return str;
        }



    }
    /*End of class TargetQueue */


    public class Test
    {
        public static void testfunction()
        {
            List<RssiValue> mylist = new List<RssiValue>();
            RssiValue val1 = new RssiValue(1, 28);
            RssiValue val2 = new RssiValue(0, 20);
            mylist.Add(val1);
            mylist.Add(val2);
            Console.Out.WriteLine(val1.toString());
            Console.Out.WriteLine(val2.toString());

            List<RssiValue> mylist2 = new List<RssiValue>();
            RssiValue val3 = new RssiValue(1, 17);
            RssiValue val4 = new RssiValue(0, 10);
            RssiValue val5 = new RssiValue(2, 10);
            mylist2.Add(val3);
            mylist2.Add(val4);
            mylist2.Add(val5);
            Console.Out.WriteLine(val3.toString());
            Console.Out.WriteLine(val4.toString());
            Console.Out.WriteLine(val5.toString());


            RssiValues values = new RssiValues();
            values.setValuesForNode(0, mylist, false);
            values.setValuesForNode(3, mylist2, false);
            Console.Out.WriteLine(values.toString());

            values.setValuesForNode(0, mylist2, true);
            Console.Out.WriteLine(values.toString());


            TargetQueue targetQ = new TargetQueue();

            targetQ.addTarget(0);
            Console.Out.WriteLine("current_target:" + targetQ.getTarget());
            Console.Out.WriteLine(targetQ.toString());

            targetQ.addTarget(0);
            Console.Out.WriteLine("current_target:" + targetQ.getTarget());
            Console.Out.WriteLine(targetQ.toString());

            targetQ.addTarget(1);
            Console.Out.WriteLine("current_target:" + targetQ.getTarget());
            Console.Out.WriteLine(targetQ.toString());

            targetQ.removeTarget(0);
            Console.Out.WriteLine("current_target:" + targetQ.getTarget());
            Console.Out.WriteLine(targetQ.toString());

            int[,] matrix = { { 1, 2, 11 }, { 3, 4, 12 }, { 5, 6, 13 }, { 7, 8, 14 }, { 9, 10, 15 } };
            RssiValues.printMatrix(matrix);
            matrix = values.getRssiValuesMatrix();
            RssiValues.printMatrix(matrix);

        }

    }

}
