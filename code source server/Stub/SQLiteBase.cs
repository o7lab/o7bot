namespace Stub
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Runtime.InteropServices;
    using System.Text;

    public class SQLiteBase
    {
        private IntPtr database;
        private const int SQL_DONE = 0x65;
        private const int SQL_OK = 0;
        private const int SQL_ROW = 100;

        public SQLiteBase()
        {
            this.database = IntPtr.Zero;
        }

        public SQLiteBase(string baseName)
        {
            this.OpenDatabase(baseName);
        }

        public void CloseDatabase()
        {
            if (this.database != IntPtr.Zero)
            {
                sqlite3_close(this.database);
            }
        }

        public void ExecuteNonQuery(string query)
        {
            IntPtr ptr;
            sqlite3_exec(this.database, this.StringToPointer(query), IntPtr.Zero, IntPtr.Zero, out ptr);
            if (ptr != IntPtr.Zero)
            {
                throw new Exception("Error with executing non-query: \"" + query + "\"!\n" + this.PointerToString(sqlite3_errmsg(ptr)));
            }
        }

        public DataTable ExecuteQuery(string query)
        {
            IntPtr ptr;
            IntPtr ptr2;
            sqlite3_prepare_v2(this.database, this.StringToPointer(query), this.GetPointerLenght(this.StringToPointer(query)), out ptr, out ptr2);
            DataTable table = new DataTable();
            for (int i = this.ReadFirstRow(ptr, ref table); i == 100; i = this.ReadNextRow(ptr, ref table))
            {
            }
            sqlite3_finalize(ptr);
            return table;
        }

        private int GetPointerLenght(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return 0;
            }
            return lstrlen(ptr);
        }

        [DllImport("kernel32")]
        private static extern IntPtr GetProcessHeap();
        public ArrayList GetTables()
        {
            string query = "SELECT name FROM sqlite_master WHERE type IN ('table','view') AND name NOT LIKE 'sqlite_%'UNION ALL SELECT name FROM sqlite_temp_master WHERE type IN ('table','view') ORDER BY 1";
            DataTable table = this.ExecuteQuery(query);
            ArrayList list = new ArrayList();
            foreach (DataRow row in table.Rows)
            {
                list.Add(row.ItemArray[0].ToString());
            }
            return list;
        }

        [DllImport("kernel32")]
        private static extern IntPtr HeapAlloc(IntPtr heap, uint flags, uint bytes);
        [DllImport("kernel32")]
        private static extern int lstrlen(IntPtr str);
        public void OpenDatabase(string baseName)
        {
            if (sqlite3_open(this.StringToPointer(baseName), out this.database) != 0)
            {
                this.database = IntPtr.Zero;
                throw new Exception("Error with opening database " + baseName + "!");
            }
        }

        private string PointerToString(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }
            Encoding encoding = Encoding.UTF8;
            int pointerLenght = this.GetPointerLenght(ptr);
            byte[] destination = new byte[pointerLenght];
            Marshal.Copy(ptr, destination, 0, pointerLenght);
            return encoding.GetString(destination, 0, pointerLenght);
        }

        private int ReadFirstRow(IntPtr statement, ref DataTable table)
        {
            table = new DataTable("resultTable");
            if (sqlite3_step(statement) == 100)
            {
                int num2 = sqlite3_column_count(statement);
                string columnName = "";
                object[] values = new object[num2];
                for (int i = 0; i < num2; i++)
                {
                    columnName = this.PointerToString(sqlite3_column_name(statement, i));
                    switch (sqlite3_column_type(statement, i))
                    {
                        case 1:
                            table.Columns.Add(columnName, Type.GetType("System.Int32"));
                            values[i] = sqlite3_column_int(statement, i);
                            break;

                        case 2:
                            table.Columns.Add(columnName, Type.GetType("System.Single"));
                            values[i] = sqlite3_column_double(statement, i);
                            break;

                        case 3:
                            table.Columns.Add(columnName, Type.GetType("System.String"));
                            values[i] = this.PointerToString(sqlite3_column_text(statement, i));
                            break;

                        case 4:
                            table.Columns.Add(columnName, Type.GetType("System.String"));
                            values[i] = this.PointerToString(sqlite3_column_blob(statement, i));
                            break;

                        default:
                            table.Columns.Add(columnName, Type.GetType("System.String"));
                            values[i] = "";
                            break;
                    }
                }
                table.Rows.Add(values);
            }
            return sqlite3_step(statement);
        }

        private int ReadNextRow(IntPtr statement, ref DataTable table)
        {
            int num = sqlite3_column_count(statement);
            object[] values = new object[num];
            for (int i = 0; i < num; i++)
            {
                switch (sqlite3_column_type(statement, i))
                {
                    case 1:
                        values[i] = sqlite3_column_int(statement, i);
                        break;

                    case 2:
                        values[i] = sqlite3_column_double(statement, i);
                        break;

                    case 3:
                        values[i] = this.PointerToString(sqlite3_column_text(statement, i));
                        break;

                    case 4:
                        values[i] = this.PointerToString(sqlite3_column_blob(statement, i));
                        break;

                    default:
                        values[i] = "";
                        break;
                }
            }
            table.Rows.Add(values);
            return sqlite3_step(statement);
        }

        [DllImport("sqlite3")]
        private static extern int sqlite3_close(IntPtr database);
        [DllImport("sqlite3")]
        private static extern IntPtr sqlite3_column_blob(IntPtr statement, int columnNumber);
        [DllImport("sqlite3")]
        private static extern int sqlite3_column_count(IntPtr statement);
        [DllImport("sqlite3")]
        private static extern double sqlite3_column_double(IntPtr statement, int columnNumber);
        [DllImport("sqlite3")]
        private static extern int sqlite3_column_int(IntPtr statement, int columnNumber);
        [DllImport("sqlite3")]
        private static extern IntPtr sqlite3_column_name(IntPtr statement, int columnNumber);
        [DllImport("sqlite3")]
        private static extern IntPtr sqlite3_column_table_name(IntPtr statement, int columnNumber);
        [DllImport("sqlite3")]
        private static extern IntPtr sqlite3_column_text(IntPtr statement, int columnNumber);
        [DllImport("sqlite3")]
        private static extern int sqlite3_column_type(IntPtr statement, int columnNumber);
        [DllImport("sqlite3")]
        private static extern IntPtr sqlite3_errmsg(IntPtr database);
        [DllImport("sqlite3")]
        private static extern int sqlite3_exec(IntPtr database, IntPtr query, IntPtr callback, IntPtr arguments, out IntPtr error);
        [DllImport("sqlite3")]
        private static extern int sqlite3_finalize(IntPtr handle);
        [DllImport("sqlite3")]
        private static extern int sqlite3_open(IntPtr fileName, out IntPtr database);
        [DllImport("sqlite3")]
        private static extern int sqlite3_prepare_v2(IntPtr database, IntPtr query, int length, out IntPtr statement, out IntPtr tail);
        [DllImport("sqlite3")]
        private static extern int sqlite3_step(IntPtr statement);
        private IntPtr StringToPointer(string str)
        {
            if (str == null)
            {
                return IntPtr.Zero;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            int num = bytes.Length + 1;
            IntPtr destination = HeapAlloc(GetProcessHeap(), 0, (uint) num);
            Marshal.Copy(bytes, 0, destination, bytes.Length);
            Marshal.WriteByte(destination, bytes.Length, 0);
            return destination;
        }

        public enum SQLiteDataTypes
        {
            BLOB = 4,
            FLOAT = 2,
            INT = 1,
            NULL = 5,
            TEXT = 3
        }
    }
}

