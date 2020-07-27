using System;


namespace WindowsFormsApp1
{
    partial class Form1
    {
        private void AkkuratToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool didpause = false;
            if (isrunning)
            {
                Button1_Click(sender, e);
                didpause = true;
            }

            designer_delegate = UpdateColorAll;
            for (int i = 0; i < 900; i++)
            {
                buttons[i].BackColor = ByteToColor(bytegrid[i]);
            }
            if (didpause)
                Button1_Click(sender, e);
        }

        private void ÜberpopulierenMöglichToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (ResetGame())
                {
                    GameMode_label_Name.Text = "Modified (Easy Overpopulation)";
                    ModifiedGameSettingsChange();
                    gamemode = RuleSetModifiedHardToBurst;
                }
            }
            else
            {
                GameMode_label_Name.Text = "Modified (Easy Overpopulation)";
                ModifiedGameSettingsChange();
                gamemode = RuleSetModifiedHardToBurst;
            }
        }

        private void CodeTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (ResetGame())
                {
                    GameMode_label_Name.Text = "Modified (Code Testing)";
                    ModifiedGameSettingsChange();
                    gamemode = RuleSetModifiedCodeTesting;
                }
            }
            else
            {
                GameMode_label_Name.Text = "Modified (Code Testing)";
                ModifiedGameSettingsChange();
                gamemode = RuleSetModifiedCodeTesting;
            }
        }

        private void KlassischToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (ResetGame())
                {
                    version = 0;
                    gamemode = RuleSetOriginal;
                    GameMode_label_Name.Text = "Klassisch";
                    //YAxisComboBox.= "# Am Leben";
                    


                }
            }
            else
            {
                version = 0;
                gamemode = RuleSetOriginal;
                GameMode_label_Name.Text = "Klassisch";
                //YAxis_labelName.Text = "# Am Leben";
               

            }
        }


        private void ModifiziertMainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (ResetGame())
                {
                    //YAxis_labelName.Text = "Summe der Gradienten";
                    GameMode_label_Name.Text = "Modified (Main)";
                    ModifiedGameSettingsChange();
                    gamemode = RuleSetModifiedMain;
                }
            }
            else
            {
               // YAxisComboBox.TextChanged += 
                //YAxis_labelName.Text = "Summe der Gradienten";
                GameMode_label_Name.Text = "Modified (Main)";
                ModifiedGameSettingsChange();
                gamemode = RuleSetModifiedMain;
            }
        }

        private void HungryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (ResetGame())
                {
                    GameMode_label_Name.Text = "Modified (High Hunger)";
                    ModifiedGameSettingsChange();
                    gamemode = RuleSetModifiedHungry;
                }
            }
            else
            {
                GameMode_label_Name.Text = "Modified (High Hunger)";
                ModifiedGameSettingsChange();
                gamemode = RuleSetModifiedHungry;
            }
        }

        private void LowHungerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (ResetGame())
                {
                    GameMode_label_Name.Text = "Modified (Low Hunger)";
                    ModifiedGameSettingsChange();
                    gamemode = RuleSetModifiedLowHunger;
                }
            }
            else
            {
                GameMode_label_Name.Text = "Modified (Low Hunger)";
                ModifiedGameSettingsChange();
                gamemode = RuleSetModifiedLowHunger;
            }
        }

        private void ShelterToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (gameexists)
            {
                if (ResetGame())
                {
                    GameMode_label_Name.Text = "Modified (Shelter)";
                    ModifiedGameSettingsChange();
                    gamemode = RuleSetModifiedShelter;
                }
            }
            else
            {
                GameMode_label_Name.Text = "Modified (Shelter)";
                ModifiedGameSettingsChange();
                gamemode = RuleSetModifiedShelter;
            }
            
        }


        private void FloaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (!ResetGame())
                {
                    return;
                }
            }

            TemplateFloaterReduced();
            gameexists = true;

        }

        private void BlinkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (!ResetGame())
                {
                    return;
                }
            }

            TemplateBlinkerReduced();
            gameexists = true;
            
        }

        private void UhrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (!ResetGame())
                {
                    return;
                }
            }

            TemplateUhrReduced();
            gameexists = true;
        }

        private void KröteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (!ResetGame())
                {
                    return;
                }
            }

            TemplateKröteReduced();
            gameexists = true;
        }

        private void BipoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (!ResetGame())
                {
                    return;
                }
            }

            TemplateBipoleReduced();
            gameexists = true;
        }

        private void TripoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (!ResetGame())
                {
                    return;
                }
            }

            TemplateTripoleReduced();
            gameexists = true;
        }

        private void PulsatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (!ResetGame())
                {
                    return;
                }
            }

            TemplatePulsatorReduced();
            gameexists = true;
        }

        private void TrümmlerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (!ResetGame())
                {
                    return;
                }
            }

            TemplateTrümmlerReduced();
            gameexists = true;
        }

        private void OktagonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (!ResetGame())
                {
                    return;
                }
            }

            TemplateOktagonReduced();
            gameexists = true;
        }
        private void LightWeightSpaceshipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (!ResetGame())
                {
                    return;
                }
            }

            TemplateLWSSReduced();
            gameexists = true;
        }

        private void MiddleWeightSpaceshipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (!ResetGame())
                {
                    return;
                }
            }

            TemplateMWSSReduced();
            gameexists = true;
        }

        private void HeavyWeightSpaceshipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (!ResetGame())
                {
                    return;
                }
            }

            TemplateHWSSReduced();
            gameexists = true;
        }

        //private void floaterToolStripMenuItem1_Click(object sender, EventArgs e)
        //{
        //    if (gameexists)
        //    {
        //        if (!ResetGame())
        //        {
        //            return;
        //        }
        //    }

        //    TemplateFloaterShelter();
        //    gameexists = true;
        //}

        private void FloaterToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (gameexists)
            {
                if (!ResetGame())
                {
                    return;
                }
            }

            TemplateFloaterShelter();
            gameexists = true;
        }
    }
}
