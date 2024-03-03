using esercizioS17L3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;


namespace esercizioS17L3.Controllers
{
    public class ShoeController : Controller
    {
        string connString = "Server=LAPTOP-4EULLOI7\\SQLEXPRESS;Initial Catalog=TennisStore;Integrated Security=True; TrustServerCertificate=True";

        private Shoe GetShoeById(int id)
        {
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();

                var command = new SqlCommand("SELECT * FROM Shoes WHERE ID = @Id", conn);
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Shoe
                        {
                            ID = (int)reader["ID"],
                            Name = reader["Name"].ToString(),
                            Price = (decimal)reader["Price"],
                            Description = reader["Description"].ToString(),
                            CoverImage = reader["CoverImage"].ToString(),
                            AdditionalImage1 = reader["AdditionalImage1"].ToString(),
                            AdditionalImage2 = reader["AdditionalImage2"].ToString(),
                            IsAvailable = (bool)reader["IsAvailable"],
                            IsDeleted = (bool)reader["IsDeleted"]
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        // GET: Shoe
        public ActionResult Index()
        {
            List<Shoe> shoes = new List<Shoe>();

            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    var command = new SqlCommand("SELECT * FROM Shoes WHERE IsDeleted = 0", conn);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var shoe = new Shoe
                            {
                                ID = (int)reader["ID"],
                                Name = reader["Name"].ToString(),
                                Price = (decimal)reader["Price"],
                                Description = reader["Description"].ToString(),
                                CoverImage = reader["CoverImage"].ToString(),
                                AdditionalImage1 = reader["AdditionalImage1"].ToString(),
                                AdditionalImage2 = reader["AdditionalImage2"].ToString(),
                                IsAvailable = (bool)reader["IsAvailable"],
                                IsDeleted = (bool)reader["IsDeleted"]
                            };
                            shoes.Add(shoe);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return View("Error");
            }

            ViewBag.IsAdmin = true;
            return View(shoes);
        }


        // GET: Shoe/Details/5
        public ActionResult Details(int id)
        {
            Shoe shoe = null;
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    var command = new SqlCommand("SELECT * FROM Shoes WHERE ID = @Id", conn);
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            shoe = new Shoe
                            {
                                ID = (int)reader["ID"],
                                Name = reader["Name"].ToString(),
                                Price = (decimal)reader["Price"],
                                Description = reader["Description"].ToString(),
                                CoverImage = reader["CoverImage"].ToString(),
                                AdditionalImage1 = reader["AdditionalImage1"].ToString(),
                                AdditionalImage2 = reader["AdditionalImage2"].ToString(),
                                IsAvailable = (bool)reader["IsAvailable"],
                                IsDeleted = (bool)reader["IsDeleted"]
                            };
                        }
                    }
                }

                if (shoe == null)
                {
                    ViewBag.ErrorMessage = "La scarpa richiesta non è stata trovata.";
                }
            }
            catch (Exception ex)
            {

                return View("Error");
            }

            return View(shoe);
        }


        // GET: Shoe/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Shoe/Create
        [HttpPost]
        public ActionResult Create(Shoe shoe, IFormFile CoverImage, IFormFile AdditionalImage1, IFormFile AdditionalImage2)
        {
            var error = true;
            var conn = new SqlConnection(connString);
            try
            {

                conn.Open();


                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // Salva la CoverImage
                string fileCoverImageName = Path.GetFileName(CoverImage.FileName);
                string fullFilePath = Path.Combine(path, fileCoverImageName);
                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    CoverImage.CopyTo(stream);
                }

                // Salva AdditionalImage1
                string fileAddImage1Name = Path.GetFileName(AdditionalImage1.FileName);
                string fullFilePath1 = Path.Combine(path, fileAddImage1Name);
                using (var stream1 = new FileStream(fullFilePath1, FileMode.Create))
                {
                    AdditionalImage1.CopyTo(stream1);
                }

                // Salva AdditionalImage2
                string fileAddImage2Name = Path.GetFileName(AdditionalImage2.FileName);
                string fullFilePath2 = Path.Combine(path, fileAddImage2Name);
                using (var stream2 = new FileStream(fullFilePath2, FileMode.Create))
                {
                    AdditionalImage2.CopyTo(stream2);
                }




                var command = new SqlCommand(
                        @"INSERT INTO Shoes (Name, Price, Description, CoverImage, AdditionalImage1, AdditionalImage2, IsAvailable, IsDeleted)
            VALUES (@Name, @Price, @Description, @CoverImage, @AdditionalImage1, @AdditionalImage2, @IsAvailable, @IsDeleted)", conn);

                command.Parameters.AddWithValue("@Name", shoe.Name);
                command.Parameters.AddWithValue("@Price", shoe.Price);
                command.Parameters.AddWithValue("@Description", shoe.Description);
                command.Parameters.AddWithValue("@CoverImage", fileCoverImageName);
                command.Parameters.AddWithValue("@AdditionalImage1", fileAddImage1Name);
                command.Parameters.AddWithValue("@AdditionalImage2", fileAddImage2Name);
                command.Parameters.AddWithValue("@IsAvailable", shoe.IsAvailable);
                command.Parameters.AddWithValue("@IsDeleted", shoe.IsDeleted);

                command.ExecuteNonQuery();
                error = false;


            }
            catch (Exception ex)
            {

                return View("Error");
            }

            return RedirectToAction("Index");
        }



        // GET: Shoe/Edit/5
        public ActionResult Edit(int id)
        {
            Shoe shoe = null;

            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    var command = new SqlCommand("SELECT * FROM Shoes WHERE ID = @Id", conn);
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            shoe = new Shoe
                            {
                                ID = (int)reader["ID"],
                                Name = reader["Name"].ToString(),
                                Price = (decimal)reader["Price"],
                                Description = reader["Description"].ToString(),
                                CoverImage = reader["CoverImage"].ToString(),
                                AdditionalImage1 = reader["AdditionalImage1"].ToString(),
                                AdditionalImage2 = reader["AdditionalImage2"].ToString(),
                                IsAvailable = (bool)reader["IsAvailable"],
                                IsDeleted = (bool)reader["IsDeleted"]
                            };
                        }
                    }
                }

                if (shoe == null)
                {
                    ViewBag.ErrorMessage = "La scarpa richiesta non è stata trovata.";
                }
            }
            catch (Exception ex)
            {

                return View("Error");
            }

            return View(shoe);
        }

        // POST: Shoe/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, Shoe shoe, IFormFile? CoverImage, IFormFile AdditionalImage1, IFormFile AdditionalImage2)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var conn = new SqlConnection(connString))
                    {
                        conn.Open();

                        // Carica le nuove immagini solo se sono state fornite
                        if (CoverImage != null)
                        {
                            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                            string fileCoverImageName = Path.GetFileName(CoverImage.FileName);
                            string fullFilePath = Path.Combine(path, fileCoverImageName);
                            using (var stream = new FileStream(fullFilePath, FileMode.Create))
                            {
                                CoverImage.CopyTo(stream);
                            }
                            // Aggiorna il percorso dell'immagine nel modello
                            shoe.CoverImage = fileCoverImageName;
                        }

                        if (AdditionalImage1 != null)
                        {
                            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                            string fileAddImage1Name = Path.GetFileName(AdditionalImage1.FileName);
                            string fullFilePath1 = Path.Combine(path, fileAddImage1Name);
                            using (var stream1 = new FileStream(fullFilePath1, FileMode.Create))
                            {
                                AdditionalImage1.CopyTo(stream1);
                            }
                            shoe.AdditionalImage1 = fileAddImage1Name;
                        }

                        if (AdditionalImage2 != null)
                        {
                            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                            string fileAddImage2Name = Path.GetFileName(AdditionalImage2.FileName);
                            string fullFilePath2 = Path.Combine(path, fileAddImage2Name);
                            using (var stream2 = new FileStream(fullFilePath2, FileMode.Create))
                            {
                                AdditionalImage2.CopyTo(stream2);
                            }
                            shoe.AdditionalImage2 = fileAddImage2Name;
                        }

                        // Esegui l'aggiornamento nel database
                        var command = new SqlCommand(
                            @"UPDATE Shoes SET Name = @Name, Price = @Price, Description = @Description, 
               CoverImage = @CoverImage, AdditionalImage1 = @AdditionalImage1, AdditionalImage2 = @AdditionalImage2, 
               IsAvailable = @IsAvailable, IsDeleted = @IsDeleted WHERE ID = @Id", conn);

                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@Name", shoe.Name);
                        command.Parameters.AddWithValue("@Price", shoe.Price);
                        command.Parameters.AddWithValue("@Description", shoe.Description);
                        command.Parameters.AddWithValue("@CoverImage", shoe.CoverImage);
                        command.Parameters.AddWithValue("@AdditionalImage1", shoe.AdditionalImage1);
                        command.Parameters.AddWithValue("@AdditionalImage2", shoe.AdditionalImage2);
                        command.Parameters.AddWithValue("@IsAvailable", shoe.IsAvailable);
                        command.Parameters.AddWithValue("@IsDeleted", shoe.IsDeleted);

                        command.ExecuteNonQuery();
                    }

                    return RedirectToAction("Details", new { id = id });
                }
            }
            catch (Exception ex)
            {
                // Log dell'errore o gestione adeguata dell'eccezione
                return View("Error");
            }

            return View(shoe);
        }



        // GET: Shoe/Delete/5
        public ActionResult Delete(int id)
        {
            Shoe shoe = null;

            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    var command = new SqlCommand("SELECT * FROM Shoes WHERE ID = @Id", conn);
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            shoe = new Shoe
                            {
                                ID = (int)reader["ID"],
                                Name = reader["Name"].ToString(),
                                Price = (decimal)reader["Price"],
                                Description = reader["Description"].ToString(),
                                CoverImage = reader["CoverImage"].ToString(),
                                AdditionalImage1 = reader["AdditionalImage1"].ToString(),
                                AdditionalImage2 = reader["AdditionalImage2"].ToString(),
                                IsAvailable = (bool)reader["IsAvailable"],
                                IsDeleted = (bool)reader["IsDeleted"]
                            };
                        }
                    }
                }

                if (shoe == null)
                {
                    ViewBag.ErrorMessage = "La scarpa richiesta non è stata trovata.";
                }
            }
            catch (Exception ex)
            {

                return View("Error");
            }

            return View(shoe);
        }

        // POST: Shoe/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    var command = new SqlCommand("DELETE FROM Shoes WHERE ID = @Id", conn);
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult SoftDelete(int idToDelete)
        {
            try
            {
                var shoe = GetShoeById(idToDelete);

                if (shoe != null)
                {
                    shoe.IsDeleted = true;

                    // Modifica per passare tutti i parametri necessari a Edit
                    Edit(shoe.ID, shoe, null, null, null); // Passa null per CoverImage, AdditionalImage1 e AdditionalImage2
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }







        // Ripristino del prodotto
        [HttpPost]
        public ActionResult Restore(int id)
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    var command = new SqlCommand("UPDATE Shoes SET IsDeleted = 0 WHERE ID = @Id", conn);
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }

                return RedirectToAction("DeletedProducts");
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        // Hard Delete
        [HttpPost]
        public ActionResult HardDelete(int id)
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    var command = new SqlCommand("DELETE FROM Shoes WHERE ID = @Id", conn);
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        public ActionResult DeletedProducts()
        {
            List<Shoe> deletedShoes = new List<Shoe>();

            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    var command = new SqlCommand("SELECT * FROM Shoes WHERE IsDeleted = 1", conn);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var shoe = new Shoe
                            {
                                ID = (int)reader["ID"],
                                Name = reader["Name"].ToString(),
                                Price = (decimal)reader["Price"],
                                Description = reader["Description"].ToString(),
                                CoverImage = reader["CoverImage"].ToString(),
                                AdditionalImage1 = reader["AdditionalImage1"].ToString(),
                                AdditionalImage2 = reader["AdditionalImage2"].ToString(),
                                IsAvailable = (bool)reader["IsAvailable"],
                                IsDeleted = (bool)reader["IsDeleted"]
                            };
                            deletedShoes.Add(shoe);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }

            ViewBag.IsAdmin = true;
            return View(deletedShoes);
        }





    }
}
