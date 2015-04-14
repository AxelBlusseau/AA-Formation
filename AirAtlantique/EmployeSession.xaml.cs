using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace AirAtlantique
{
    /// <summary>
    /// Logique d'interaction pour EmployeSession.xaml
    /// </summary>
    public partial class EmployeSession : Window
    {
        public AirAtlantiqueDBContext db { get; set; }

        public EmployeSession()
        {
            db = new AirAtlantiqueDBContext();
            InitializeComponent();

            var recup = MainWindow.envoi;

            var reqES = from es in db.Employes_Sessions
                        where es.idSession == recup
                        orderby es.Employe.nom
                        select new { es.Employe.nom, es.Employe.prenom };

            var reqID = from es in db.Employes_Sessions
                        where es.idSession == recup
                        orderby es.Employe.nom
                        select es.idEmploye;

            var reqE = from e in db.Employes
                       where !reqID.Contains(e.id)
                       select new { e.nom, e.prenom};

            foreach (var item in reqES)
            {
                LBoxEmployeSession.Items.Add(new Employe { nom = item.nom, prenom = item.prenom });
            }

            foreach (var item in reqE)
            {
                LBoxEmploye.Items.Add(new Employe { nom = item.nom, prenom = item.prenom });
            }       

        }

        private void btnAjoutEmploye_Click(object sender, RoutedEventArgs e)
        {
            var recup = MainWindow.envoi;
            var reqTS = from s in db.Sessions
                        where s.idSession == recup
                        select s.nbPlace;

            if (int.Parse(LBoxEmployeSession.Items.Count.ToString()) >= reqTS.First())
            {
                MessageBox.Show("Le nombre max d'employés pour cette session est atteint");

            }
            else if (LBoxEmploye.SelectedIndex >= 0)
            {             
                LBoxEmployeSession.Items.Add(LBoxEmploye.SelectedItem);

                LBoxEmployeSession.Items.SortDescriptions.Add(new SortDescription("nom",
                                  ListSortDirection.Ascending));    

                LBoxEmploye.Items.Remove(LBoxEmploye.SelectedItem);
            }

        }

        private void btnDelEmploye_Click(object sender, RoutedEventArgs e)
        {
            if (LBoxEmployeSession.SelectedIndex >= 0)
            {
                LBoxEmploye.Items.Add(LBoxEmployeSession.SelectedItem);

                LBoxEmploye.Items.SortDescriptions.Add(new SortDescription("nom",
                                  ListSortDirection.Ascending));  

                LBoxEmployeSession.Items.Remove(LBoxEmployeSession.SelectedItem);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            var recup = MainWindow.envoi;
            
            var reqES = from es in db.Employes_Sessions
                        where es.idSession == recup
                        select es.idEmploye;

            var listReq = reqES.ToArray();

            foreach (var val in listReq)
            {
                var boolean = true;

                foreach (var item in LBoxEmployeSession.Items)
                {
                    var nom = (item as Employe).nom;
                    var prenom = (item as Employe).prenom;

                    var reqIE = db.Employes
                    .Where(ie => ie.nom == nom)
                    .Where(ie => ie.prenom == prenom)
                    .Select(ie => ie.id);


                    if (val == reqIE.First())
                    {
                        boolean = false;
                    }
                }

                if (boolean == true)
                {                  
                    var delete = db.Employes_Sessions.First(s => s.idEmploye == val);
                    
                    db.Employes_Sessions.Remove(delete);
                    db.SaveChanges();
                }
                
            }




            foreach (var item in LBoxEmployeSession.Items)
            {             
                var nom = (item as Employe).nom;
                var prenom = (item as Employe).prenom;
                var boolean = true;
                var delete = true;

                var reqIE = db.Employes
                    .Where(ie => ie.nom == nom)
                    .Where(ie => ie.prenom == prenom)
                    .Select(ie => ie.id);


                foreach (var val in reqES)
                {
                    if (val == reqIE.First())
                    {
                        boolean = false;               
                    }

                }

                 if (boolean == true)
                {

                    var empSess = new Employes_Sessions
                    {
                        idEmploye = reqIE.First(),
                        idSession = recup
                    };

                    db.Employes_Sessions.Add(empSess);
                    db.SaveChanges();

                }
            
            }

            this.Close();            
                         
        }

    }

}
