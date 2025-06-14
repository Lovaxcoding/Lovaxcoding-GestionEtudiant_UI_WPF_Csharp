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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using BoostCsharp; // Assurez-vous que ce using est bien là pour accéder à DbFunctions
using System.Data;


namespace BoostCsharp
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DbFunctions dbFunction;
        public MainWindow()
        {
            InitializeComponent();
            dbFunction = new DbFunctions();
            LoadEtudiantsIntoDataGrid();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove(); // Permet de déplacer la fenêtre
            }
        }

        // Ajoutez (ou assurez-vous de l'existence de) cette méthode
        private void btnCloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Ferme toute l'application
            // Ou this.Close(); pour fermer juste cette fenêtre si vous en avez plusieurs
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string name = TextName.Text.ToString();
            string matricule = Matricule.Text.ToString();
            string email = TextEmail.Text.ToString() + "@gmail.com";
            
            if(!string.IsNullOrWhiteSpace(name)){
                MessageBoxResult result = MessageBox.Show("Voulez-vous réellement ajouter ?", "Lova", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    // Votre méthode AjouterUtilisateur dans DbFunctions doit accepter 3 paramètres si vous voulez passer le matricule.
                    // Actuellement, la version que je vous ai donnée n'en prend que deux (nom, email).
                    // Il faudra modifier DbFunctions.AjouterUtilisateur pour accepter le matricule.
                    // Je vais vous montrer comment juste après.
                    bool success = dbFunction.AjouterUtilisateur(name, email, matricule);

                    if (success)
                    {
                        MessageBox.Show("Utilisateur ajouté avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Videz les champs après l'ajout pour une meilleure UX
                        TextName.Text = "";
                        Matricule.Text = "";
                        TextEmail.Text = "";
                        LoadEtudiantsIntoDataGrid();
                        // Appelez une fonction pour rafraîchir un DataGrid si vous en avez un.
                        // Exemple: LoadUsersIntoDataGrid(); 
                    }
                    else
                    {
                        // Le message d'erreur est déjà géré par la classe DbFunctions via MessageBox.Show
                        // donc vous n'avez pas forcément besoin d'un autre ici, sauf pour un message générique.
                        // MessageBox.Show("Échec de l'ajout de l'utilisateur.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Verifier l'erreur dans votre code");
            }

        }

         private void dataGridEtudiants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ancienne syntaxe compatible avec les versions antérieures de C#
            if (dataGridEtudiants.SelectedItem != null)
            {
                DataRowView selectedRow = dataGridEtudiants.SelectedItem as DataRowView;
                if (selectedRow != null) // Vérifier que le cast a réussi
                {
                    // Récupère les données de la ligne sélectionnée et les affiche dans les TextBoxes
                    TextName.Text = selectedRow["Name"].ToString();
                    Matricule.Text = selectedRow["Matricule"].ToString();
                    string fullEmail = selectedRow["Email"].ToString();
                    if (fullEmail.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
                    {
                        TextEmail.Text = fullEmail.Substring(0, fullEmail.Length - "@gmail.com".Length);
                    }
                    else
                    {
                        TextEmail.Text = fullEmail;
                    }
                }
            }
            else
            {
                // Si aucune ligne n'est sélectionnée, videz les TextBoxes
                TextName.Text = "";
                Matricule.Text = "";
                TextEmail.Text = "";
            }
        }
        private void LoadEtudiantsIntoDataGrid()
        {
            // Appelle la méthode de votre classe DbFunctions pour obtenir toutes les personnes (étudiants)
            DataTable etudiantsTable = dbFunction.GetTousLesUtilisateurs();

            // Vérifie si des données ont été retournées (et si l'objet est non null)
            if (etudiantsTable != null)
            {
                // Lie le DataTable à la propriété ItemsSource du DataGrid
                // Assurez-vous que le nom 'dataGridEtudiants' correspond à 'x:Name' dans votre XAML
                dataGridEtudiants.ItemsSource = etudiantsTable.DefaultView;
            }
            else
            {
                // Gérer le cas où la récupération des données a échoué (ex: afficher un message d'erreur)
                MessageBox.Show("Impossible de charger les données des étudiants. Vérifiez la connexion à la base de données.", "Erreur de chargement", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnMiseAJour_Click(object sender, RoutedEventArgs e)
        {
            // Ancienne syntaxe compatible avec les versions antérieures de C#
            if (dataGridEtudiants.SelectedItem != null)
            {
                DataRowView selectedRow = dataGridEtudiants.SelectedItem as DataRowView;
                if (selectedRow != null) // Vérifier que le cast a réussi
                {
                    int idToUpdate = Convert.ToInt32(selectedRow["Id"]);

                    string newName = TextName.Text.Trim();
                    string newMatricule = Matricule.Text.Trim();
                    string newEmailPrefix = TextEmail.Text.Trim();
                    string newEmail = newEmailPrefix + "@gmail.com";

                    if (!string.IsNullOrWhiteSpace(newName) && !string.IsNullOrWhiteSpace(newMatricule) && !string.IsNullOrWhiteSpace(newEmailPrefix))
                    {
                        MessageBoxResult confirmUpdate = MessageBox.Show("Confirmez-vous la mise à jour de l'étudiant {newName} (ID: {idToUpdate}) ?", "Confirmation de mise à jour", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (confirmUpdate == MessageBoxResult.Yes)
                        {
                            bool success = dbFunction.MettreAJourUtilisateur(idToUpdate, newName, newEmail, newMatricule);

                            if (success)
                            {
                                MessageBox.Show("Étudiant mis à jour avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadEtudiantsIntoDataGrid();
                                TextName.Text = "";
                                Matricule.Text = "";
                                TextEmail.Text = "";
                            }
                            // else : message d'erreur déjà géré par DbFunctions
                        }
                    }
                    else
                    {
                        MessageBox.Show("Veuillez remplir tous les champs (Nom, Matricule, Email) pour la mise à jour.", "Champs manquants", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else // Ce else est pour le cas où le SelectedItem n'est pas un DataRowView
                {
                    MessageBox.Show("Erreur interne : l'élément sélectionné n'est pas du bon type.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else // Ce else est pour le cas où aucun élément n'est sélectionné
            {
                MessageBox.Show("Veuillez sélectionner un étudiant à mettre à jour dans la liste.", "Aucune sélection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void LoadEtudiantsIntoDataGrid(string searchTerm = null) // Ajout d'un paramètre optionnel
        {
            DataTable etudiantsTable;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Si un terme de recherche est fourni, utilisez la méthode de recherche
                etudiantsTable = dbFunction.RechercherUtilisateurs(searchTerm);
            }
            else
            {
                // Sinon, chargez tous les utilisateurs
                etudiantsTable = dbFunction.GetTousLesUtilisateurs();
            }

            if (etudiantsTable != null)
            {
                dataGridEtudiants.ItemsSource = etudiantsTable.DefaultView;
            }
            else
            {
                MessageBox.Show("Impossible de charger les données des étudiants. Vérifiez la connexion à la base de données.", "Erreur de chargement", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // --- NOUVELLE MÉTHODE : Gestionnaire pour le bouton "Rechercher" ---
        private void RechercheBTN_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = RechercheText.Text.Trim(); // Récupère le texte de recherche

            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadEtudiantsIntoDataGrid(searchTerm); // Charge les données filtrées
            }
            else
            {
                // Si le champ de recherche est vide, rechargez toutes les données
                LoadEtudiantsIntoDataGrid();
                MessageBox.Show("Le champ de recherche est vide. Affichage de tous les étudiants.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Optionnel : Rechercher en temps réel (au fur et à mesure de la frappe)
        // Vous pouvez attacher cet événement au TextChanged du RechercheText
        private void RechercheText_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Désactivez temporairement ce code si les performances sont un problème avec beaucoup de données
            // ou si vous préférez une recherche uniquement via le bouton.
            string searchTerm = RechercheText.Text.Trim();
            if (!string.IsNullOrEmpty(searchTerm) || e.Changes.Any(c => c.AddedLength > 0 || c.RemovedLength > 0))
            {
                LoadEtudiantsIntoDataGrid(searchTerm);
            }
            else if (string.IsNullOrEmpty(searchTerm) && dataGridEtudiants.ItemsSource != (dbFunction.GetTousLesUtilisateurs()).DefaultView)
            {
                // Si le champ est vidé, rechargez toutes les données
                LoadEtudiantsIntoDataGrid();
            }
        }

    


        private void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            // Ancienne syntaxe compatible avec les versions antérieures de C#
            if (dataGridEtudiants.SelectedItem != null)
            {
                DataRowView selectedRow = dataGridEtudiants.SelectedItem as DataRowView;
                if (selectedRow != null) // Vérifier que le cast a réussi
                {
                    int idToDelete = Convert.ToInt32(selectedRow["Id"]);
                    string nameToDelete = selectedRow["Name"].ToString();

                    MessageBoxResult confirmDelete = MessageBox.Show("Voulez-vous vraiment supprimer l'étudiant {nameToDelete} (ID: {idToDelete}) ?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (confirmDelete == MessageBoxResult.Yes)
                    {
                        bool success = dbFunction.SupprimerUtilisateur(idToDelete);

                        if (success)
                        {
                            MessageBox.Show("Étudiant supprimé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadEtudiantsIntoDataGrid();
                            TextName.Text = "";
                            Matricule.Text = "";
                            TextEmail.Text = "";
                        }
                        // else : message d'erreur déjà géré par DbFunctions
                    }
                }
                else // Ce else est pour le cas où le SelectedItem n'est pas un DataRowView
                {
                    MessageBox.Show("Erreur interne : l'élément sélectionné n'est pas du bon type.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else // Ce else est pour le cas où aucun élément n'est sélectionné
            {
                MessageBox.Show("Veuillez sélectionner un étudiant à supprimer dans la liste.", "Aucune sélection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
       
            
    }
}
