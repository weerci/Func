using Func;

namespace FuncTest
{
    public class FileServiceTest
    {
        string wrongPath = "cc:\\zero";
        string rootPath = "c:\\zero.txt";
        string rightPath = "c:\\1\\zero.txt";
        string emptyDir = "";

        [Fact]
        public void Non_exist_file()
        {
            var fn = FileName.Open(wrongPath);

            Assert.True(fn.IsException);
            Assert.Equal("Unable to find the specified file.", fn.Error?.Message);
        }

        [Fact]
        public void Path_is_wrong()
        {
            var fn = FileName.OpenOrCreate(wrongPath);

            Assert.True(fn.IsException);
            Assert.True(fn.Error?.Message.Contains("Синтаксическая ошибка в имени файла"));
        }

        [Fact]
        public void Create_root_non_exist_file()
        {
            var fn = FileName.OpenOrCreate(rootPath);

            Assert.True(fn.IsException);
            Assert.True(fn.Error?.Message.Contains("Access to the path"));
        }

        [Fact]
        public void Create_non_exist_file_with_path_and_delete()
        {
            var fn = FileName.OpenOrCreate(rightPath);

            try
            {
                Assert.Equal("c:\\1", fn.Value?.Dir);
                Assert.Equal(rightPath, fn.Value?.PathFile);
                Assert.Equal(".txt", fn.Value?.Ext);
                Assert.Equal("zero.txt", fn.Value?.NameExt);
                Assert.Equal("zero", fn.Value?.Name);
            }
            finally
            {
                var dir = fn.Value?.Path.GetDirectoryName(rightPath);

                Assert.True(fn.IsSuccess);
                fn.Value?.File.Delete(rightPath);

                Assert.False(fn.Value?.File.Exists(rightPath));

                fn.Value?.Directory.Delete(dir!);
                Assert.False(fn.Value?.File.Exists(rightPath));
            }
        }

        [Fact]
        public void Create_non_exist_file_with_empty_dir()
        {
            var fn = FileName.OpenOrCreate(emptyDir + "zero.txt");

            Assert.True(fn.Exists());

            fn.Delete();
            Assert.False(fn.Exists());

        }

    }
}