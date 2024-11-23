namespace EAD.Configuration
{
    /// <summary>
    /// Represents the settings required for connecting to a MongoDB database.
    /// </summary>
    public class MongoDBSettings
    {
        /// <summary>
        /// Gets or sets the connection string used to connect to the MongoDB server.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the database to use within the MongoDB server.
        /// </summary>
        public string DatabaseName { get; set; }
    }
}
