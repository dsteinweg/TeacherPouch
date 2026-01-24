module Database

open FSharp.Data.Sql

type DB = SqlDataProvider<
            Common.DatabaseProviderTypes.SQLITE,
            @"C:\Source\TeacherPouch\data\TeacherPouch.sqlite",
            SQLiteLibrary=Common.SQLiteLibrary.MicrosoftDataSqlite,
            ResolutionPath = @"C:\Source\TeacherPouch\bin\sqlite\win",
            CaseSensitivityChange = Common.CaseSensitivityChange.ORIGINAL
          >
