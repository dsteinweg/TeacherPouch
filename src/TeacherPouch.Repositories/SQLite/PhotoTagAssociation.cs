using System;
using System.Collections.Generic;
using System.Linq;
using DbExtensions;

using TeacherPouch.Models;

namespace TeacherPouch.Repositories.SQLite
{
    internal class PhotoTagAssociation
    {
        private const string PHOTOTAG_TABLE_NAME = "Photo_Tag";
        private const string PHOTOTAG_COLUMN_NAMES = "PhotoID, TagID";

        public int PhotoID { get; set; }
        public int TagID { get; set; }


        internal static List<PhotoTagAssociation> GetAll()
        {
            var query = SQL.SELECT(PHOTOTAG_COLUMN_NAMES)
                           .FROM(PHOTOTAG_TABLE_NAME);

            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                return connection.Map<PhotoTagAssociation>(query).ToList();
            }
        }

        internal static List<PhotoTagAssociation> GetAll(Photo photo)
        {
            var query = SQL.SELECT(PHOTOTAG_COLUMN_NAMES)
                           .FROM(PHOTOTAG_TABLE_NAME)
                           .WHERE("PhotoID = {0}", photo.ID);

            List<PhotoTagAssociation> associations = null;
            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                associations = connection.Map<PhotoTagAssociation>(query).ToList();
            }

            return associations;
        }

        internal static List<PhotoTagAssociation> GetAll(Tag tag)
        {
            var query = SQL.SELECT(PHOTOTAG_COLUMN_NAMES)
                           .FROM(PHOTOTAG_TABLE_NAME)
                           .WHERE("TagID = {0}", tag.ID);

            List<PhotoTagAssociation> associations = null;
            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                associations = connection.Map<PhotoTagAssociation>(query).ToList();
            }

            return associations;
        }


        internal static void EnsurePhotoTagAssociation(Photo photo, Tag tag)
        {
            var query = SQL.SELECT(PHOTOTAG_COLUMN_NAMES)
                           .FROM(PHOTOTAG_TABLE_NAME)
                           .WHERE("PhotoID = {0}", photo.ID)
                           ._("TagID = {0}", tag.ID);

            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                var existingAssociation = connection.Map<PhotoTagAssociation>(query).FirstOrDefault();
                if (existingAssociation == null)
                {
                    var insertQuery = SQL.INSERT_INTO(String.Format("{0}({1})", PHOTOTAG_TABLE_NAME, PHOTOTAG_COLUMN_NAMES))
                                         .VALUES(photo.ID, tag.ID);

                    connection.Execute(insertQuery);
                }
            }
        }

        internal static void DeletePhotoAssociationsForTag(Tag tag)
        {
            var query = SQL.DELETE_FROM(PHOTOTAG_TABLE_NAME)
                           .WHERE("TagID = {0}", tag.ID);

            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                connection.Execute(query);
            }
        }

        internal static void DeleteTagAssociationsForPhoto(Photo photo)
        {
            var query = SQL.DELETE_FROM(PHOTOTAG_TABLE_NAME)
                           .WHERE("PhotoID = {0}", photo.ID);

            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                connection.Execute(query);
            }
        }
    }
}
