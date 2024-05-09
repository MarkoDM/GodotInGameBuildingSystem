using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Godot.GodotInGameBuildingSystem;

/// <summary> Provides functionality to save and load game data. </summary>
/// <remarks> This is a generic class that can be used to save and load any type of data. </remarks>
public static class SaveSystem
{
    /// <summary> Determines whether encryption is enabled for saving data. </summary>
    /// <remarks> Encryption is enabled by default to prevent tempering with save data outside the game. </remarks>
    private static readonly bool useEncryption = false;

    /// <summary> The file extension used for saving data. </summary>
    /// <remarks>
    /// The default extension is "json". 
    /// Extension can be whatever or nothing, for example ".sav"
    /// When encryption is disabled, put "json" for readability as json is used to serialize object to a file.
    /// </remarks>
    private const string saveExtension = "json";

    /// <summary> The folder where save files are stored. </summary>
    /// <remarks> 
    /// The default folder is "user://".
    /// For reference https://docs.godotengine.org/en/stable/tutorials/io/data_paths.html#accessing-persistent-user-data-user
    /// </remarks>
    private static readonly string saveFolder = "user://";

    /// <summary> The base file name used for saving data. </summary>
    private static readonly string baseFileName = "savegame";

    /// <summary> Generates a unique save file name based on the current date and time. </summary>
    /// <returns> The generated save file name. </returns>
    private static string GenerateSaveFileName() => $"{saveFolder}{baseFileName}_{DateTime.Now.ToFileTime()}.{saveExtension}";

    /// <summary> Saves the specified data to a file. </summary>
    /// <remarks>
    /// It uses JsonSerializer to serialize the data to a JSON string. Be sure to provide valid data types.
    /// </remarks>
    /// <typeparam name="T"> The type of data to save. </typeparam>
    /// <param name="savefile"> The data to save. </param>
    /// <param name="saveFileName"> The name of the save file. If not provided, a unique name will be generated. </param>
    /// <returns> The name of the saved file. </returns>
    public static string Save<T>(T savefile, string saveFileName = null)
    {
        saveFileName = saveFileName == null ? GenerateSaveFileName() : saveFolder + saveFileName;
        using var saveGame = FileAccess.Open(saveFileName, FileAccess.ModeFlags.Write);
        if (FileAccess.GetOpenError() != Error.Ok)
        {
            GD.PrintErr(FileAccess.GetOpenError());
            return saveFileName;
        }
        try
        {
            var jsonString = JsonSerializer.Serialize(savefile);
            if (useEncryption) jsonString = CryptoUtils.EncryptString(jsonString);
            saveGame.StoreString(jsonString);
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
        }
        return saveFileName;
    }

    /// <summary> Loads data from the specified save file. </summary>
    /// <remarks>
    /// It uses JsonSerializer to deserialize the data.
    /// </remarks>
    /// <typeparam name="T"> The type of data to load. </typeparam>
    /// <param name="saveFileName"> The name of the save file to load. </param>
    /// <returns> The loaded data. </returns>
    public static T Load<T>(string saveFileName)
    {
        using var saveGame = FileAccess.Open(saveFolder + saveFileName, FileAccess.ModeFlags.Read);
        if (FileAccess.GetOpenError() != Error.Ok)
        {
            GD.PrintErr(FileAccess.GetOpenError());
            return default;
        }
        try
        {
            var jsonString = saveGame.GetAsText();
            if (useEncryption) jsonString = CryptoUtils.DecryptString(jsonString);
            var saveFile = JsonSerializer.Deserialize<T>(jsonString);
            return saveFile;
        }
        catch (Exception e)
        {
            if (e is FormatException) GD.PrintErr("Save file is not encrypted, disable encryption to load.");
            else GD.PrintErr(e);
        }
        return default;
    }

    /// <summary> Loads the most recent save file. </summary>
    /// <typeparam name="T"> The type of data to load. </typeparam>
    /// <returns> The loaded data from the most recent save file. </returns>
    public static T LoadMostRecentFile<T>()
    {
        var saveFiles = GetSaveFilesInfo();
        if (saveFiles.Count > 0)
        {
            return Load<T>(saveFiles.FirstOrDefault().Name);
        }
        return default;
    }

    /// <summary> Retrieves information about all the save files in the save folder. </summary>
    /// <returns> A list of <see cref="FileInfo"/> objects representing the save files. </returns>
    public static List<FileInfo> GetSaveFilesInfo()
    {
        var directoryPath = ProjectSettings.GlobalizePath("user://");
        if (!Directory.Exists(directoryPath))
        {
            GD.Print($"Directory does not exist: {directoryPath}");
            return new List<FileInfo>();
        }

        DirectoryInfo dirInfo = new(directoryPath);
        FileInfo[] files = dirInfo.GetFiles();

        return files.OrderByDescending(f => f.LastWriteTime).ToList();
    }
}
