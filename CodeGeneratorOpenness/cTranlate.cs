using System;
using System.Windows.Forms;

namespace CodeGeneratorOpenness
{
    public class cTranlate
    {
        public cTranlate(Form Form, string Culture)
        {
            string txt = string.Empty;
            foreach (Control c in Form.Controls)
            {
                // save the text
                if ((c.Text != string.Empty) && (c.Tag == null))
                    c.Tag = c.Text;

                txt = (string)c.Tag;

                if (c is Label)
                {
                    if (Culture == "DE")
                    {
                        if (txt == "Data Type") c.Text = "Datentyp";
                        else if (txt == "Safety Data Block") c.Text = "Sicherer Datenbaustein";
                        else if (txt == "Safety Fuction Block") c.Text = "Sicherer Funktionsblock";
                        else if (txt == "Safety Organization Block") c.Text = "Sicherer Organisationsblock";
                        else if (txt == "Data Block") c.Text = "Datenbaustein";
                        else if (txt == "Function") c.Text = "Funktion";
                        else if (txt == "Function Block") c.Text = "Funktionsblock";
                        else if (txt == "Organization Block") c.Text = "Organisationsblock";
                        else if (txt == "Blocks") c.Text = "Blöcke";
                        else if (txt == "PLC") c.Text = "SPS";
                        else if (txt == "Device") c.Text = "Station";
                        else
                            Console.WriteLine(txt);
                    }
                    else
                    {
                        // EN
                        if (txt != string.Empty) c.Text = txt;
                    }
                }

                if (c is Button)
                {
                    if (Culture == "DE")
                    {
                        if (txt == "Reload") c.Text = "neu Laden";
                        else if (txt == "Open project") c.Text = "Projekt öffnen";
                        else
                            Console.WriteLine(txt);
                    }
                    else
                    {
                        // EN
                        if (txt != string.Empty) c.Text = txt;
                    }
                }

                if (c is MenuStrip)
                {
                    MenuStrip strip = (MenuStrip)c;
                    foreach (ToolStripMenuItem m in strip.Items)
                    {
                        // save the text
                        if ((m.Text != string.Empty) && (m.Tag == null))
                            m.Tag = m.Text;

                        txt = (string)m.Tag;

                        if (Culture == "DE")
                        {
                            if (txt == "Project") m.Text = "Projekt";
                            else if (txt == "Import") m.Text = "Import";
                            else if (txt == "Export") m.Text = "Export";
                            else if (txt == "Project Texts") m.Text = "Projekt Texte";
                            else if (txt == "Language") m.Text = "Sprache";
                            else
                                Console.WriteLine(txt);
                        }
                        else
                        {
                            // EN
                            if (txt != string.Empty) m.Text = txt;
                        }

                        foreach (ToolStripItem n in m.DropDownItems)
                        {
                            // save the text
                            if ((n.Text != string.Empty) && (n.Tag == null))
                                n.Tag = n.Text;

                            txt = (string)n.Tag;

                            if (Culture == "DE")
                            {
                                if (txt == "Open") n.Text = "Öffnen";
                                else if (txt == "Compile") n.Text = "Kompilieren";
                                else if (txt == "Save") n.Text = "Speichern";
                                else if (txt == "Close") n.Text = "Schließen";
                                else if (txt == "Exit") n.Text = "Ende";
                                else if (txt == "PLC Blocks") n.Text = "SPS Bausteine";
                                else if (txt == "PLC Data types") n.Text = "SPS Daten Typen";
                                else if (txt == "Export") n.Text = "Export";
                                else if (txt == "Import") n.Text = "Import";
                                else if (txt == "English") n.Text = "Englisch";
                                else if (txt == "German") n.Text = "Deutsch";
                                else
                                    Console.WriteLine(txt);
                            }
                            else
                            {
                                // EN
                                if (txt != string.Empty) n.Text = txt;
                            }
                        }
                    }
                }
            }
        }

        public void TranslateContext(ContextMenu Context, string Culture)
        {
            foreach (MenuItem m in Context.MenuItems)
            {
                // save the text
                if ((m.Text != string.Empty) && (m.Tag == null))
                    m.Tag = m.Text;

                string txt = (string)m.Tag;

                if (Culture == "DE")
                {
                    if (txt == "Delete") m.Text = "Löschen";
                    else if (txt == "Add group") m.Text = "Neue Gruppe";
                    else if (txt == "Delete group") m.Text = "Gruppe löschen";
                    else
                        Console.WriteLine(txt);
                }
                else
                {
                    // EN
                    if (txt != string.Empty) m.Text = txt;
                }
            }
        }
    }
}
