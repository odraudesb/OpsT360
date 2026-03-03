using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using ApiModels.AppModels;
using System.Threading.Tasks;
//using ApiModels.Resultados;

namespace BRBKApp.DA
{
    public class Data
    {
        readonly SQLiteAsyncConnection database;

        public Data(string dbpath)
        {
            database = new SQLiteAsyncConnection(dbpath);

            database.CreateTableAsync<UserDB>().Wait();
            database.CreateTableAsync<TasksBD>().Wait();
            

        }

        public Task<int> RegistroDatos(UserDB NewDato)
        {
            return database.InsertAsync(NewDato);
        }

        public Task<int> UpdateDatos(UserDB UpdDato)
        {
            return database.UpdateAsync(UpdDato);
        }

        public Task<int> DeleteDatos(UserDB DelDato)
        {
            return database.DeleteAsync(DelDato);
        }

        public Task<List<UserDB>> GetRegistrado()
        {
            var lista = database.Table<UserDB>().ToListAsync();

            return lista;
        }

        public Task<List<UserDB>> GetRegistradoById(long Idregistrado)
        {
            var lista = database.Table<UserDB>().Where(z => z.UserId == Idregistrado).ToListAsync();

            return lista;
        }

        public Task<int> RegistroTareas(TasksBD NewDato)
        {
            return database.InsertAsync(NewDato);
        }

        public Task<int> UpdateTareas(TasksBD UpdDato)
        {
            return database.UpdateAsync(UpdDato);
        }

        public Task<int> DeleteTareas(TasksBD DelDato)
        {
            return database.DeleteAsync(DelDato);
        }

        public Task<List<TasksBD>> GetRegistradoTareas()
        {
            var lista = database.Table<TasksBD>().ToListAsync();

            return lista;
        }

        public Task<List<TasksBD>> GetRegistradoTareasById(int Idregistrado)
        {
            var lista = database.Table<TasksBD>().Where(z => z.Id_Word == Idregistrado).ToListAsync();

            return lista;
        }

        
        //public Task<int> RegistroFotos(RegistradoFotos NewDato)
        //{
        //    return database.InsertAsync(NewDato);
        //}

        //public Task<List<RegistradoFotos>> GetRegistradoFotosById(int Idregistrado)
        //{
        //    var lista = database.Table<RegistradoFotos>().Where(z => z.IdRegistrado == Idregistrado).ToListAsync();

        //    return lista;
        //}

    }
}
