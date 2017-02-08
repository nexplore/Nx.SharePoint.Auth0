using System.IO;

namespace Auth0.Core.Http
{

    /// <summary>
    /// 
    /// </summary>
    public class FileUploadParameter
    {
        //TODO: RWM: See if it is possible to make this class internal. It's used by an interface so I don't know if it can be.

        public string Key { get; set; }
        public string Filename { get; set; }
        public Stream FileStream { get; set; }

    }

}