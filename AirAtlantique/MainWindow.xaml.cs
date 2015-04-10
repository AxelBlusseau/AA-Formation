using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AirAtlantique
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AirAtlantiqueDBContext db { get; set; }

        public MainWindow()
        {
            db = new AirAtlantiqueDBContext();
            InitializeComponent();
                     
            var req = from f in db.Formations select f.nom;

            foreach (var item in req)
            {
                LBFormation.Items.Add(item);
            }
        }

        private void AjoutF_Click(object sender, RoutedEventArgs e)
        {

            if (TBAjoutF.Text != "")
            {
                
                var formation = new Formation()
                {
                    nom = TBAjoutF.Text
                };

                db.Formations.Add(formation);
                db.SaveChanges();

                LBFormation.Items.Add(TBAjoutF.Text);
                
            }
            else
            {
                MessageBox.Show("Veuillez entrer un nom de formation avant de l'ajouter");
            }


        }

        private void SelectSession(object sender, RoutedEventArgs e)
        {

            try
            {      
                LBSession.Items.Clear();
                TBNbPlace.Clear();
                TBDuree.Clear();
                DPSession.Text = "";

           
                var req = from f in db.Formations
                            join s in db.Sessions
                            on f.idFormation equals s.idFormation
                            where f.nom == LBFormation.SelectedItem.ToString()
                            select s.nom;

                foreach (var item in req)
                {
                    LBSession.Items.Add(item);
                }
            }
            catch (Exception)
            {

            }
        }

        private void SelectInfosSession(object sender, RoutedEventArgs e)
        {
            try
            {
                var req = from s in db.Sessions
                            where s.nom == LBSession.SelectedItem.ToString()
                            select new { s.nom, s.date, s.duree, s.nbPlace };

                foreach (var item in req)
                {
                    TBNbPlace.Text = item.nbPlace.ToString();
                    TBDuree.Text = item.duree.ToString();
                    DPSession.Text = item.date.ToString();
                }
            }
            catch (Exception) { }

        }

        private void btnModif_Click(object sender, RoutedEventArgs e)
        {
            var modif = db.Sessions.First(s => s.nom == LBSession.SelectedItem.ToString());
            modif.nbPlace = int.Parse(TBNbPlace.Text);
            modif.duree = int.Parse(TBDuree.Text);
            modif.date = Convert.ToDateTime(DPSession.Text);

            db.SaveChanges();    
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

            if (LBSession.SelectedIndex >= 0)
            {
                    MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Etes-vous sûr de vouloir supprimer cette session ?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    var delete = db.Sessions.First(s => s.nom == LBSession.SelectedItem.ToString());

                    db.Sessions.Remove(delete);

                    db.SaveChanges();

                    LBSession.Items.Clear();
                    TBNbPlace.Clear();
                    TBDuree.Clear();
                    DPSession.Text = "";

                    var req = from f in db.Formations
                                join s in db.Sessions
                                on f.idFormation equals s.idFormation
                                where f.nom == LBFormation.SelectedItem.ToString()
                                select s.nom;


                    foreach (var item in req)
                    {
                        LBSession.Items.Add(item);
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner une session pour pouvoir la supprimer");
            }
        }
        private void btnAjoutSess_Click(object sender, RoutedEventArgs e)
        {

            if (LBFormation.SelectedIndex >= 0)
            {
                var req = from f in db.Formations
                            where f.nom == LBFormation.SelectedItem.ToString()
                            select f.idFormation;

                var session = new Session()
                {
                    nom = TBoxAjoutSess.Text,
                    date = Convert.ToDateTime(DPAjoutSession.Text),
                    duree = int.Parse(TBoxAjoutDuree.Text),
                    nbPlace = int.Parse(TBoxAjoutNbPlace.Text),
                    idFormation = req.First()
                };


                db.Sessions.Add(session);
                db.SaveChanges();

                LBSession.Items.Add(TBoxAjoutSess.Text);

            }
            else
                MessageBox.Show("Veuillez Selectionner une Formation");
        }

        private void ButtonDeleteF_Click(object sender, RoutedEventArgs e)
        {


            if (LBFormation.SelectedIndex >= 0)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Etes-vous sûr de vouloir supprimer cette formation ? (Toutes les sessions associées seront supprimer)", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                        

                    var req3 = from f in db.Formations
                                where f.nom == LBFormation.SelectedItem.ToString()
                                select f.idFormation;

                    if (db.Sessions.First(s => s.idFormation == req3.FirstOrDefault()) == )
                    {

                    }
                    var delete2 = db.Sessions.First(s => s.idFormation == req3.FirstOrDefault());
                    db.Sessions.Remove(delete2);
                    db.SaveChanges();
                    
                    var delete = db.Formations.First(f => f.nom == LBFormation.SelectedItem.ToString());

                    db.Formations.Remove(delete);
                    db.SaveChanges();

                    var req = from f in db.Formations
                                select f.nom;

                    LBFormation.Items.Clear();
                    foreach (var item in req)
                    {
                        LBFormation.Items.Add(item);
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner une session pour pouvoir la supprimer");
            }
        }
    }
}
