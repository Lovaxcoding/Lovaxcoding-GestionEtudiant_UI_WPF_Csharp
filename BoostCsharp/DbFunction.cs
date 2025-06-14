using System;
using System.Configuration; // Pour ConfigurationManager
using System.Data;          // Pour DataTable
using System.Data.SqlClient; // Pour SqlConnection, SqlCommand, SqlDataReader, etc.
using System.Windows;       // Pour MessageBox (utile pour les messages d'erreur/succès dans une app WPF)

namespace BoostCsharp // Assurez-vous que le namespace correspond à celui de votre projet
{
    public class DbFunctions
    {
        private string connectionString;

        // Constructeur : Initialise la chaîne de connexion au moment de la création de l'objet DbFunctions
        public DbFunctions()
        {
            try
            {
                // Récupère la chaîne de connexion depuis App.config
                connectionString = ConfigurationManager.ConnectionStrings["MaConnexionBoostCsharp"].ConnectionString;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la récupération de la chaîne de connexion : {ex.Message}\nAssurez-vous que 'MaConnexionBoostCsharp' est définie dans App.config et que la référence System.Configuration est ajoutée.", "Erreur de configuration", MessageBoxButton.OK, MessageBoxImage.Error);
                // Vous pouvez aussi logguer l'erreur ou la relancer selon votre besoin
                throw; // Rejeter l'exception pour que l'application ne continue pas sans connexion
            }
        }

        // --- Opération : CREATE (Insérer une nouvelle donnée) ---
        public bool AjouterUtilisateur(string nom, string email, string matricule)
        {
            // Requête SQL paramétrée pour éviter les injections SQL
            string query = "INSERT INTO Personne (Name, Email, Matricule) VALUES (@Nom, @Email, @Matricule)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Ajout des paramètres
                    command.Parameters.AddWithValue("@Nom", nom);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Matricule", matricule);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery(); // Retourne le nombre de lignes affectées

                        if (rowsAffected > 0)
                        {
                            //MessageBox.Show("Utilisateur ajouté avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                            return true;
                        }
                        else
                        {
                            //MessageBox.Show("Aucun utilisateur ajouté.", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Erreur SQL lors de l'ajout de l'utilisateur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Une erreur inattendue s'est produite lors de l'ajout : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
            }
        }

        // --- Opération : READ (Lire des données) ---
        // Cette fonction retourne un DataTable, facile à lier à un DataGrid en WPF
        public DataTable GetTousLesUtilisateurs()
        {
            DataTable dt = new DataTable();
            string query = "SELECT Id, Name, Email, Matricule FROM Personne"; // Sélectionnez les colonnes que vous voulez afficher

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dt.Load(reader); // Charge toutes les données du SqlDataReader dans le DataTable
                        }
                        //MessageBox.Show("Données des utilisateurs récupérées avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Erreur SQL lors de la récupération des utilisateurs : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Une erreur inattendue s'est produite lors de la récupération : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            return dt;
        }

        public DataTable RechercherUtilisateurs(string motCle)
{
    DataTable dt = new DataTable();
    // La requête utilise LIKE pour la recherche partielle et '%' pour les jokers.
    // Elle cherche dans les colonnes Name, Matricule et Email.
    // UPPER() ou LOWER() est utilisé pour une recherche insensible à la casse.
    // ATTENTION : Le nom de votre table est [dbo].[Personne], pas 'Utilisateurs'
    string query = "SELECT Id, Name, Email, Matricule FROM Personne " +
                   "WHERE UPPER(Name) LIKE UPPER(@MotCle) OR " +
                   "      UPPER(Matricule) LIKE UPPER(@MotCle) OR " +
                   "      UPPER(Email) LIKE UPPER(@MotCle)";

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            // Nous ajoutons des jokers '%' autour du mot-clé pour rechercher des correspondances partielles.
            command.Parameters.AddWithValue("@MotCle", "%" + motCle + "%");
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Erreur SQL lors de la recherche des utilisateurs : "+ ex.Message, "Erreur de base de données", MessageBoxButton.OK, MessageBoxImage.Error);
                return null; // Retourne null en cas d'erreur
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur inattendue s'est produite lors de la recherche : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return null; // Retourne null en cas d'erreur
            }
        }
    }
    return dt;
}

        // --- Opération : UPDATE (Mettre à jour une donnée existante) ---
        public bool MettreAJourUtilisateur(int id, string nouveauNom, string nouvelEmail, string nouvelMatricule)
        {
            string query = "UPDATE Personne SET Name = @NouveauNom, Email = @NouvelEmail, Matricule = @NouvelMatricule WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NouveauNom", nouveauNom);
                    command.Parameters.AddWithValue("@NouvelEmail", nouvelEmail);
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@NouvelMatricule", nouvelMatricule);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            //MessageBox.Show("Utilisateur mis à jour avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                            return true;
                        }
                        else
                        {
                            //MessageBox.Show("Aucun utilisateur mis à jour (ID introuvable ou pas de changement).", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Erreur SQL lors de la mise à jour de l'utilisateur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Une erreur inattendue s'est produite lors de la mise à jour : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
            }
        }

        // --- Opération : DELETE (Supprimer une donnée) ---
        public bool SupprimerUtilisateur(int id)
        {
            string query = "DELETE FROM Personne WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            //MessageBox.Show("Utilisateur supprimé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                            return true;
                        }
                        else
                        {
                            //MessageBox.Show("Aucun utilisateur supprimé (ID introuvable).", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Erreur SQL lors de la suppression de l'utilisateur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Une erreur inattendue s'est produite lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
            }
        }

        // --- Méthode utilitaire : Tester la connexion ---
        public bool TesterLaConnexion()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MessageBox.Show("Connexion à la base de données réussie !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Erreur SQL lors du test de connexion : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Une erreur inattendue lors du test de connexion : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }
    }
}