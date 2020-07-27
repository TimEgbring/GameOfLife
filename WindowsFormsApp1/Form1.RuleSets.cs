﻿using System;
namespace WindowsFormsApp1
{
    partial class Form1
    {
        private void RuleSetModifiedCodeTesting()
        {
            for (int i = 0; i < 900; i++)
            {
                if (hasaliveneighbors[i] || isalive[i])
                {       //Calculates Bytegrid_new

                    if (neighbors_gradient_sum[i] == 12 || neighbors_gradient_sum[i] == 16 || neighbors_gradient_sum[i] == 20)
                    {
                        bytegrid_new[i] = bytegrid[i];
                    }
                    else if (neighbors_gradient_sum[i] < 7 || neighbors_gradient_sum[i] > 12) //Too small or too big
                    {

                        if (isalive[i])
                        {

                            bytegrid_new[i]--;
                        }
                    }
                    else if (neighbors_gradient_sum[i] > 9 && neighbors_gradient_sum[i] < 12) //Just right
                    {
                        if (bytegrid[i] != 4)
                        {

                            bytegrid_new[i]++;

                        }

                        isalive[i] = true;
                    }



                    else //enough to survive
                    {
                        bytegrid_new[i] = bytegrid[i];
                    }
                }




            }
            BytegridChangeAction();
            for (int i = 0; i < 900; i++)
            {
                bytegrid[i] = bytegrid_new[i];
            }
            designer_delegate(); //UpdateColorAll() or UpdateColorAllDesigner
            for (int i = 0; i < 900; i++)
            {
                designer_bytegrid_haschanged[i] = false;
            }


        }
        private void RuleSetOriginal()
        {
            for (int i = 0; i < 900; i++)
            {
                if (hasaliveneighbors[i] || isalive[i])
                {       //Calculates Bytegrid_new
                    if (aliveneighbors_count[i] <= 1 || aliveneighbors_count[i] >= 4)
                    {
                        bytegrid_new[i] = 0;
                        isalive[i] = false;
                    }
                    else if (aliveneighbors_count[i] == 3)
                    {
                        bytegrid_new[i] = 4;
                        isalive[i] = true;
                    }
                    else
                    {
                        bytegrid_new[i] = bytegrid[i];
                    }
                }
            }
            ReducedBytegridChangeAction();
            for (int i = 0; i < 900; i++)
            {
                bytegrid[i] = bytegrid_new[i];
            }
            ReducedUpdateColorAll();
        }
        private void RuleSetModifiedHungry()
        {
            for (int i = 0; i < 900; i++)
            {
                if (hasaliveneighbors[i] || isalive[i])
                {       //Calculates Bytegrid_new
                    if (neighbors_gradient_sum[i] < 9 || neighbors_gradient_sum[i] > 12) //Too small or too big
                    {
                        if (isalive[i])
                        {
                            bytegrid_new[i]--;
                        }
                    }
                    else if (neighbors_gradient_sum[i] > 9 && neighbors_gradient_sum[i] < 12) //Just right
                    {
                        if (bytegrid[i] != 4)
                        {
                            bytegrid_new[i]++;
                        }
                        isalive[i] = true;
                    }
                    else //enough to survive
                    {
                        bytegrid_new[i] = bytegrid[i];
                    }
                }
            }
            BytegridChangeAction();
            for (int i = 0; i < 900; i++)
            {
                bytegrid[i] = bytegrid_new[i];
            }
            UpdateColorAll();
        }
        private void RuleSetModifiedLowHunger()
        {
            for (int i = 0; i < 900; i++)
            {
                if (hasaliveneighbors[i] || isalive[i])
                {       //Calculates Bytegrid_new
                    if (neighbors_gradient_sum[i] < 7 || neighbors_gradient_sum[i] > 12) //Too small or too big
                    {
                        if (isalive[i])
                        {
                            bytegrid_new[i]--;
                        }
                    }
                    else if (neighbors_gradient_sum[i] > 7 && neighbors_gradient_sum[i] < 12) //Just right
                    {
                        if (bytegrid[i] != 4)
                        {
                            bytegrid_new[i]++;
                        }
                        isalive[i] = true;
                    }
                    else //enough to survive
                    {
                        bytegrid_new[i] = bytegrid[i];
                    }
                }
            }
            BytegridChangeAction();
            for (int i = 0; i < 900; i++)
            {
                bytegrid[i] = bytegrid_new[i];
            }
            UpdateColorAll();
        }
        private void RuleSetModifiedHardToBurst()
        {
            for (int i = 0; i < 900; i++)
            {
                if (hasaliveneighbors[i] || isalive[i])
                {       //Calculates Bytegrid_new
                    if (neighbors_gradient_sum[i] < 8 || neighbors_gradient_sum[i] > 13) //Too small or too big
                    {
                        if (isalive[i])
                        {
                            bytegrid_new[i]--;
                        }
                    }
                    else if (neighbors_gradient_sum[i] > 8 && neighbors_gradient_sum[i] < 13) //Just right
                    {
                        if (bytegrid[i] != 4)
                        {
                            bytegrid_new[i]++;
                        }
                        isalive[i] = true;
                    }
                    else //enough to survive
                    {
                        bytegrid_new[i] = bytegrid[i];
                    }
                }
            }
            BytegridChangeAction();
            for (int i = 0; i < 900; i++)
            {
                bytegrid[i] = bytegrid_new[i];
            }
            UpdateColorAll();
        }
        private void RuleSetModifiedMain()
        {
            for (int i = 0; i < 900; i++)
            {
                if (hasaliveneighbors[i] || isalive[i])
                {       //Calculates Bytegrid_new
                    if (neighbors_gradient_sum[i] < 8 || neighbors_gradient_sum[i] > 12) //Too small or too big
                    {
                        if (isalive[i])
                        {
                            bytegrid_new[i]--;
                        }
                    }
                    else if (neighbors_gradient_sum[i] > 8 && neighbors_gradient_sum[i] < 12) //Just right
                    {
                        if (bytegrid[i] != 4)
                        {
                            bytegrid_new[i]++;
                        }
                        isalive[i] = true;
                    }
                    else //enough to survive
                    {
                        bytegrid_new[i] = bytegrid[i];
                    }
                }
            }
            BytegridChangeAction();
            for (int i = 0; i < 900; i++)
            {
                bytegrid[i] = bytegrid_new[i];
            }
            UpdateColorAll();
        }

        private void RuleSetModified6_9()
        {
            for (int i = 0; i < 900; i++)
            {
                if (hasaliveneighbors[i] || isalive[i])
                {       //Calculates Bytegrid_new
                    if (neighbors_gradient_sum[i] < 6 || neighbors_gradient_sum[i] > 9) //Too small or too big
                    {
                        if (isalive[i])
                        {
                      
                            bytegrid_new[i]--;
                        }
                    }
                    else if (neighbors_gradient_sum[i] > 6 && neighbors_gradient_sum[i] < 9) //Just right
                    {
                        if (bytegrid[i] != 4)
                        {
                            bytegrid_new[i]++;
                     
                        }
                        isalive[i] = true;
                    }
                    else //enough to survive
                    {
                        bytegrid_new[i] = bytegrid[i];
                    }
                }
            }
            BytegridChangeAction();
            for (int i = 0; i < 900; i++)
            {
                bytegrid[i] = bytegrid_new[i];
            }
            UpdateColorAll();
        }


       

        private void RuleSetModifiedShelter()
        {

            for (int i = 0; i < 900; i++)
            {
               
                bool first;
                bool second;
                bool third;
                //bytegrid_new[i] = (byte)(
                //    Convert.ToInt16((first = !(hasaliveneighbors[i] || isalive[i]) || neighbors_gradient_sum[i] == 12 || neighbors_gradient_sum[i] == 16 || neighbors_gradient_sum[i] == 20) && (bytegrid_new[i] = bytegrid[i])
                //    + Convert.ToInt16((second= !first && isalive[i] && (neighbors_gradient_sum[i] < 8 || neighbors_gradient_sum[i] > 12))) * (bytegrid[i] -1)
                //    + Convert.ToInt16((third = !first && bytegrid[i] != 4 && neighbors_gradient_sum[i] > 8 && neighbors_gradient_sum[i] < 12) && (isalive[i] = true)) * (bytegrid[i] + 1)
                //    + Convert.ToInt16(!first && !second && !third) * (bytegrid[i])

                //    );

                first = (!(hasaliveneighbors[i] || isalive[i]) || neighbors_gradient_sum[i] == 12 || neighbors_gradient_sum[i] == 16 || neighbors_gradient_sum[i] == 20) && ((bytegrid_new[i] = bytegrid[i]) == bytegrid_new[i]);
                second = (!first && isalive[i] && (neighbors_gradient_sum[i] < 8 || neighbors_gradient_sum[i] > 12)) && (bytegrid_new[i] = (byte)(bytegrid[i] - 1)) == bytegrid_new[i];
                third = (!first && bytegrid[i] != 4 && neighbors_gradient_sum[i] > 8 && neighbors_gradient_sum[i] < 12) && ((isalive[i] = true) && (bytegrid_new[i] = (byte)(bytegrid[i] + 1)) ==bytegrid_new[i]);
                third = (!first && !second && !third) && (bytegrid_new[i] = bytegrid[i]) == bytegrid_new[i];



            }
            BytegridChangeAction();
            for (int i = 0; i < 900; i++)
            {
                bytegrid[i] = bytegrid_new[i];
            }
            designer_delegate(); //UpdateColorAll() or UpdateColorAllDesigner
            for (int i = 0; i < 900; i++)
            {
                designer_bytegrid_haschanged[i] = false;
            }


        }
        private void SAVERuleSetModifiedShelter()
        {

            for (int i = 0; i < 900; i++)
            {
                if (hasaliveneighbors[i] || isalive[i])
                {       //Calculates Bytegrid_new

                    if (neighbors_gradient_sum[i] == 12 || neighbors_gradient_sum[i] == 16 || neighbors_gradient_sum[i] == 20)
                    {
                        bytegrid_new[i] = bytegrid[i];
                    }
                    else if (neighbors_gradient_sum[i] < 8 || neighbors_gradient_sum[i] > 12) //Too small or too big
                    {

                        if (isalive[i])
                        {

                            bytegrid_new[i]--;
                        }
                    }
                    else if (neighbors_gradient_sum[i] > 8 && neighbors_gradient_sum[i] < 12) //Just right
                    {
                        if (bytegrid[i] != 4)
                        {

                            bytegrid_new[i]++;

                        }

                        isalive[i] = true;
                    }



                    else //enough to survive
                    {
                        bytegrid_new[i] = bytegrid[i];
                    }
                }




            }
            BytegridChangeAction();
            for (int i = 0; i < 900; i++)
            {
                bytegrid[i] = bytegrid_new[i];
            }
            designer_delegate(); //UpdateColorAll() or UpdateColorAllDesigner
            for (int i = 0; i < 900; i++)
            {
                designer_bytegrid_haschanged[i] = false;
            }


        }
    }
}
