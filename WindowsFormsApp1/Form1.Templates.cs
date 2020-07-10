

namespace WindowsFormsApp1
{
    partial class Form1
    {

        private void SimulatePressButtonReduced(int buttonnumber)
        {
            if (((BorderlessButton)buttons[buttonnumber]).BackColor == dead_white)
            {
                ((BorderlessButton)buttons[buttonnumber]).BackColor = full_black;
                bytegrid[buttonnumber] = 4;
                IsAliveAndStatInc(buttonnumber);
                AliveNeighborsIncBool(buttonnumber);
            }
            else
            {
                ((BorderlessButton)buttons[buttonnumber]).BackColor = dead_white;
                bytegrid[buttonnumber] = 0;
                AliveNeighborsDecBool(buttonnumber);
                IsNotAliveAndStatDec(buttonnumber);
            }
        }

        private void SimulatePressButton(int buttonnumber)
        {
            if (((BorderlessButton)buttons[buttonnumber]).BackColor == dead_white) //Modified
            {
                ((BorderlessButton)buttons[buttonnumber]).BackColor = mygrey1;
                bytegrid[buttonnumber] = 1;
                
                IsAliveAndStatInc(buttonnumber);
                AliveNeighborsIncBool(buttonnumber);
                NeighborGradientSumInc(buttonnumber);
            }
            else if (((BorderlessButton)buttons[buttonnumber]).BackColor == mygrey1)
            {
                ((BorderlessButton)buttons[buttonnumber]).BackColor = mygrey2;
                bytegrid[buttonnumber] = 2;
              
                NeighborGradientSumInc(buttonnumber);

            }
            else if (((BorderlessButton)buttons[buttonnumber]).BackColor == mygrey2)
            {
                ((BorderlessButton)buttons[buttonnumber]).BackColor = mygrey3;
                bytegrid[buttonnumber] = 3;
               
                NeighborGradientSumInc(buttonnumber);

            }
            else if (((BorderlessButton)buttons[buttonnumber]).BackColor == mygrey3)
            {
                ((BorderlessButton)buttons[buttonnumber]).BackColor = full_black;
                bytegrid[buttonnumber] = 4;
               
                NeighborGradientSumInc(buttonnumber);

            }
            else if (((BorderlessButton)buttons[buttonnumber]).BackColor == full_black)
            {
                ((BorderlessButton)buttons[buttonnumber]).BackColor = dead_white;
                bytegrid[buttonnumber] = 0;
             
                AliveNeighborsDecBool(buttonnumber);
                IsNotAliveAndStatDec(buttonnumber);
                NeighborGradientSumDec(buttonnumber);
            }
        }


        private void PressButtonList(int[] bnumber_list)
        {
            if(version == 0)
            foreach(int i in bnumber_list)
            {
                SimulatePressButtonReduced(i);
            }
            else foreach (int i in bnumber_list)
                {
                    SimulatePressButton(i);
                }
        }

        public int[] AddMapToList(int[] list, int shift)
        {
            
            for(int i = 0; i< list.Length; i++)
            {
                list[i] = list[i] + shift;
            }
            return list;
        }
        private void TemplateFloaterReduced()
        {
            int[] floater_reduc = { 404, 435, 465, 406, 436 };
            PressButtonList(AddMapToList(floater_reduc, 0));
        }

        private void TemplateUhrReduced()
        {
            int[] c_red = { 122,93,63,124,154,95};
            PressButtonList(AddMapToList(c_red, 283));
        }

        private void TemplateKröteReduced()
        {
            int[] c_red = { 311,341,371,342,372,402};
            PressButtonList(AddMapToList(c_red, 0));
        }

        private void TemplateBipoleReduced()
        {
            int[] c_red = { 525,555,556,526,587,617,618,588 };
            PressButtonList(AddMapToList(c_red, -145));
        }
        private void TemplateTripoleReduced()
        {
            int[] c_red = { 248,249,278,280,340,371,372,342};
            PressButtonList(AddMapToList(c_red, 5));
        }
        private void TemplatePulsatorReduced()
        {
            int[] c_red = { 460,461,432,492,463,464,465,466,437,497,468,469};
            PressButtonList(AddMapToList(c_red, 28));
        }
        private void TemplateTrümmlerReduced()
        {
            int[] c_red = { 165,196,195,226,256,254,284,285,315,258,228,198,199,169,289,319,290,260};
            PressButtonList(AddMapToList(c_red, 0));
        }
        private void TemplateOktagonReduced()
        {
            int[] c_red = { 311,371,401,461,430,340,342,432,433,343,314,374,404,464,345,435};
            PressButtonList(AddMapToList(c_red, 34));
        }

        private void TemplateBlinkerReduced()
        {
            int[] c_red = { 450,480,510};
            PressButtonList(AddMapToList(c_red, -15));
        }

        private void TemplateLWSSReduced()
        {
            int[] c_red = { 402,399,339,310,311,312,313,343,373 };
            PressButtonList(AddMapToList(c_red, 0));
        }
        private void TemplateMWSSReduced()
        {
            int[] c_red = { 402,342,313,314,315,316,317,347,377,406,434 };
            PressButtonList(AddMapToList(c_red, 0));
        }
        private void TemplateHWSSReduced()
        {
            int[] c_red = { 402,342,313,314,315,316,317,318,348,378,407,435,434 };
            PressButtonList(AddMapToList(c_red, 0));
        }

        private void TemplateFloaterShelter()
        {
            int[] c_red = { 341,342,372,372,370,370,370,370,369,369,399,400,400,400,400,401,401,401,401};
            PressButtonList(AddMapToList(c_red, 0));
        }
    }
}
