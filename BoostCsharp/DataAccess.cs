using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;
using System.Configuration;

namespace BoostCsharp
{
    class DataAccess
    {
        private string connectionString;

    public DataAccess()
    {
        // Votre chaîne de connexion récupérée depuis App.config
        connectionString = ConfigurationManager.ConnectionStrings["MaConnexionBoostCsharp"].ConnectionString;
    }

    public void TesterLaConnexion()
    {
        // Création de l'objet SqlConnection avec votre chaîne de connexion
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                // Tenter d'ouvrir la connexion
                connection.Open();
                MessageBox.Show("Connexion à la base de données réussie !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SqlException ex)
            {
                // Gérer les erreurs spécifiques à SQL Server
                MessageBox.Show("Erreur de connexion à la base de données : {ex.Message}\nCode d'erreur : {ex.ErrorCode}", "Erreur SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.Exception ex)
            {
                // Gérer les autres types d'erreurs
                MessageBox.Show("Une erreur inattendue s'est produite : {ex.Message}", "Erreur Générale", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Le bloc 'using' garantit que la connexion est fermée automatiquement, même en cas d'erreur.
        }
    }

    // Ajoutez ici vos méthodes pour SELECT, INSERT, UPDATE, DELETE...
    // Rappelez-vous d'utiliser des SqlCommand avec des paramètres pour la sécurité !


    }
}
