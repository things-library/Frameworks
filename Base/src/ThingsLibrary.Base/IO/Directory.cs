
using ThingsLibrary.DataType;

namespace ThingsLibrary.IO
{
    public static class Directory
    {
        /// <summary>
        /// This method will create the missing underlying path including sub directories
        /// </summary>
        /// <param name="directoryPath">Directory path to create / verify</param>
        /// <exception cref="ArgumentNullException">If path is empty</exception>
        public static void VerifyPath(string directoryPath)
        {
            // nothing to see here folks
            ArgumentNullException.ThrowIfNullOrEmpty(directoryPath, nameof(directoryPath));
            
            // already exists???
            if (System.IO.Directory.Exists(directoryPath)) { return; }

            var response = System.IO.Directory.CreateDirectory(directoryPath);
        }

        /// <summary>
        /// Tries to gracefully delete a folder but if error occurs it continues.
        /// </summary>
        /// <param name="directoryPath">Directory to delete</param>
        /// <returns>Success or failure to delete directory</returns>
        /// <exception cref="ArgumentNullException">If directory path is empty</exception>
        public static bool TryDeleteDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath)) { throw new ArgumentNullException(nameof(directoryPath)); }

            if (!System.IO.Directory.Exists(directoryPath)) { return false; }

            try
            {
                System.IO.Directory.Delete(directoryPath, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return !System.IO.Directory.Exists(directoryPath);
        }

        /// <summary>
        /// Recursively copies all files/directories from source path to target path
        /// </summary>
        /// <param name="sourcePath">Source path</param>
        /// <param name="destinationPath">Destination Path</param>
        /// <param name="overwrite">If files should be overwritten</param>
        public static void RecursiveCopy(string sourcePath, string destinationPath, bool overwrite)
        {
            if (string.IsNullOrEmpty(sourcePath)) { throw new ArgumentNullException(nameof(sourcePath)); }
            if (string.IsNullOrEmpty(destinationPath)) { throw new ArgumentNullException(nameof(destinationPath)); }

            if (string.Compare(sourcePath, destinationPath, true) == 0) { throw new ArgumentException("Source and destination are the same."); }

            // Check if the directories exists..
            if (!System.IO.Directory.Exists(sourcePath)) { throw new ArgumentException($"Source Path does not exist at: '{sourcePath}'"); }            
            if (!System.IO.Directory.Exists(destinationPath))
            {
                VerifyPath(destinationPath);
            }

            // Copy each file into it's new directory.
            foreach (var filePath in System.IO.Directory.GetFiles(sourcePath))
            {
                System.IO.File.Copy(filePath, Path.Combine(destinationPath, Path.GetFileName(filePath)), overwrite);
            }

            // Copy each subdirectory using recursion.
            foreach (var subDirSourcePath in System.IO.Directory.GetDirectories(sourcePath))
            {
                RecursiveCopy(subDirSourcePath, Path.Combine(destinationPath, Path.GetFileName(subDirSourcePath)), overwrite);
            }
        }

        /// <summary>
        /// Compares source file to destination.. true if all files in SOURCE match destination.. destination might have extra files
        /// </summary>
        /// <param name="sourcePath">Source Path</param>
        /// <param name="destinationPath">Destination Path</param>
        /// <returns>True if all files/folders in source are contained in the destination folder</returns>
        /// <exception cref="ArgumentException">If source path doesn't exist</exception>
        public static bool RecursiveCompare(string sourcePath, string destinationPath)
        {
            if (string.IsNullOrEmpty(sourcePath)) { throw new ArgumentNullException(nameof(sourcePath)); }
            if (string.IsNullOrEmpty(destinationPath)) { throw new ArgumentNullException(nameof(destinationPath)); }

            if (string.Compare(sourcePath, destinationPath, true) == 0) { throw new ArgumentException("Source and destination are the same."); }

            if (!System.IO.Directory.Exists(sourcePath)) { throw new ArgumentException($"Source Path does not exist at: '{sourcePath}'"); }

            // if the destination doesn't exist then nothing to compare to
            if (!System.IO.Directory.Exists(destinationPath)) { return false; }

            // Copy each file into it's new directory.
            foreach (var sourceFilePath in System.IO.Directory.GetFiles(sourcePath))
            {
                var destinationFilePath = Path.Combine(destinationPath, Path.GetFileName(sourceFilePath));
                if (!System.IO.File.Exists(destinationFilePath)) { return false;  } // file doesn't even exist

                // compare file contents
                var sourceMD5 = ThingsLibrary.IO.File.ComputeMD5Base64(sourceFilePath);
                var destinationMD5 = ThingsLibrary.IO.File.ComputeMD5Base64(destinationFilePath);
                if(string.Compare(sourceMD5, destinationMD5, false) != 0) { return false; } // found one that doesn't match the source record
            }

            // Compare each subdirectory using recursion.
            foreach (var subDirSourcePath in System.IO.Directory.GetDirectories(sourcePath))
            {
                if(!RecursiveCompare(subDirSourcePath, Path.Combine(destinationPath, Path.GetFileName(subDirSourcePath)))) { return false; }                
            }

            // if we got here we had no reason to not assume a full match
            return true;
        }
    }
}
