using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.Collections.ObjectModel;

namespace Final.QuickNote.Class
{
    public class DatabaseManager
    {
        private SQLiteConnection dataBase = null;

        public DatabaseManager(string databasePath)
        {
            dataBase = new SQLiteConnection(databasePath);
        }

        public void LoadData()
        {
            dataBase.CreateTable<Tag>();
            dataBase.CreateTable<Note>();
            dataBase.CreateTable<Note_Tag>();
        }

        #region Getter
        public IEnumerable<Tag> GetListTag()
        {
            return dataBase.Table<Tag>();
        }

        public IEnumerable<Note> GetListNote()
        {
            return dataBase.Table<Note>();
        }

        public IEnumerable<Note> GetSpecificListNote(Tag tag)
        {
            return dataBase.Query<Note>(@"select Note.* from Note, Note_Tag where Note_Tag.IdNote = Note.Id and Note_Tag.IdTag = ?", tag.Id);
        }

        public IEnumerable<Tag> GetSpecificListTag(Note note)
        {
            return dataBase.Query<Tag>(@"select Tag.* from Tag, Note_Tag where Note_Tag.IdTag = Tag.Id and Note_Tag.IdNote = ?", note.Id);
        }

        public Note GetNote(int IdNote)
        {
            return dataBase.Get<Note>(IdNote);
        }

        public Tag GetTag(int IdTag)
        {
            return dataBase.Get<Tag>(IdTag);
        }
        #endregion

        #region Updater
        public bool UpdateTag(Tag newTag)
        {
            if (dataBase.Table<Tag>().Any(x => String.Compare(x.Name, newTag.Name, true) == 0 && x.Id != newTag.Id)) return false;
            dataBase.Update(newTag);
            return true;
        }

        public void UpdateAllTag(IEnumerable<Tag> newListTag)
        {
            dataBase.UpdateAll(newListTag);
        }
        #endregion

        #region Check Exist
        public Tag CheckTagExist(Tag tag)
        {
            List<Tag> data = dataBase.Table<Tag>().ToList();
            IEnumerable<Tag> list = data.Where(v => String.Compare(v.Name, tag.Name, StringComparison.OrdinalIgnoreCase) == 0);
            if (list == null) return null;
            return list.FirstOrDefault();
        }

        public bool CheckNoteExist(int idNote)
        {
            return dataBase.Table<Note>().Any(x => x.Id == idNote);
        }
        #endregion

        #region Inserter
        public void InsertNote(Note note, ObservableCollection<Tag> tags)
        {
            dataBase.Insert(note);

            if (tags.Count == 0)
            {
                Tag otherNote = new Tag() { Name = "Other" };

                Tag res = CheckTagExist(otherNote);

                if (res == null)
                {
                    otherNote.Amount = 1;
                    res = otherNote;
                    dataBase.Insert(otherNote);
                }
                else
                {
                    res.Amount++;
                    dataBase.Update(res);
                }

                dataBase.Insert(new Note_Tag() { IdNote = note.Id, IdTag = res.Id });
            }
            else
            {
                foreach (var tag in tags)
                {
                    Tag res = CheckTagExist(tag);
                    if (res == null)
                    {
                        tag.Amount = 1;
                        res = tag;
                        dataBase.Insert(tag);
                    }
                    else
                    {
                        res.Amount++;
                        dataBase.Update(res);
                    }

                    dataBase.Insert(new Note_Tag() { IdNote = note.Id, IdTag = res.Id });
                }
            }
        }
        #endregion

        #region Modifier
        public void ModifyNote(Note note, ObservableCollection<Tag> tags)
        {
            dataBase.Update(note);

            IEnumerable<Tag> noteTags = GetSpecificListTag(note);

            foreach (var tag in noteTags)
            {
                tag.Amount--;
                dataBase.Update(tag);
            }

            dataBase.Execute(@"DELETE FROM Note_Tag WHERE IdNote = ?", note.Id);

            if (tags.Count == 0)
            {
                Tag otherNote = new Tag() { Name = "Other" };

                Tag res = CheckTagExist(otherNote);

                if (res == null)
                {
                    otherNote.Amount = 1;
                    res = otherNote;
                    dataBase.Insert(otherNote);
                }
                else
                {
                    res.Amount++;
                    dataBase.Update(res);
                }

                dataBase.Insert(new Note_Tag() { IdNote = note.Id, IdTag = res.Id });
            }
            else
            {
                foreach (var tag in tags)
                {
                    Tag res = CheckTagExist(tag);
                    if (res == null)
                    {
                        tag.Amount = 1;
                        res = tag;
                        dataBase.Insert(tag);
                    }
                    else
                    {
                        res.Amount++;
                        dataBase.Update(res);
                    }

                    dataBase.Insert(new Note_Tag() { IdNote = note.Id, IdTag = res.Id });
                }
            }
        }
        #endregion

        #region Deleter
        /// <summary>
        /// Remove note out of table Note
        /// Remove all linked this note vs any tag from table Note_Tag
        /// </summary>
        /// <param name="note"></param>
        public void DeleteNote(Note note)
        {
            //int idNote = note.Id;
            dataBase.Delete(note);

            IEnumerable<Tag> noteTags = GetSpecificListTag(note);

            foreach (var tag in noteTags)
            {
                tag.Amount--;
                dataBase.Update(tag);
            }

            dataBase.Execute(@"DELETE FROM Note_Tag WHERE IdNote = ?", note.Id);
        }

        /// <summary>
        /// If amount = 0 then just remove tag out of table Tag
        /// Otherwise, if amount > 0 then get lstNote before remove tag out of table and all of linked Note_Tag
        /// and check if note in lstNote has 0 linked Note_Tag after remove then add linked Note_Tag: this note - "Other" tag (note not belong to any tag)
        /// </summary>
        /// <param name="tag"></param>
        public void DeleteTag(Tag tag)
        {
            dataBase.Delete(tag);

            if (tag.Amount > 0)
            {
                IEnumerable<Note> lstNote = dataBase.Query<Note>(@"select Note.* from Note, Note_Tag where Note_Tag.IdNote = Note.Id and Note_Tag.IdTag = ?", tag.Id);
                dataBase.Execute(@"DELETE FROM Note_Tag WHERE IdTag = ?", tag.Id);

                foreach (var note in lstNote)
                {
                    if (dataBase.Table<Note_Tag>().Count(x => x.IdNote == note.Id) == 0)
                    {
                        Tag otherNote = new Tag() { Name = "Other" };
                        Tag res = CheckTagExist(otherNote);

                        if (res == null)
                        {
                            otherNote.Amount = 1;
                            res = otherNote;
                            dataBase.Insert(otherNote);
                        }
                        else
                        {
                            res.Amount++;
                            dataBase.Update(res);
                        }

                        dataBase.Insert(new Note_Tag() { IdNote = note.Id, IdTag = res.Id });
                    }
                }
            }
        }
        #endregion
    }
}
