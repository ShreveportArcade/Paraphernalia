// From: http://forum.unity3d.com/threads/46398-Batch-audio-clip-import-settings-modifier
// Author: Mat with fixes by luvcraft 
// Renamed menu items - Nolan Baker

using UnityEngine;
using UnityEditor;

public class AudioImportEditor {
 
    [MenuItem ("Audio/Toggle Compression/Disable")]
    static void ToggleCompressionDisable() {
        SelectedToggleCompressionSettings(AudioImporterFormat.Native);
    }
 
    [MenuItem ("Audio/Toggle Compression/Enable")]
    static void ToggleCompressionEnable() {
        SelectedToggleCompressionSettings(AudioImporterFormat.Compressed);
    }
 
    // ----------------------------------------------------------------------------
 
    [MenuItem ("Audio/Compression Bitrate (kbps)/32")]
    static void SetCompressionBitrate32kbps() {
        SelectedSetCompressionBitrate(32000);
    }
 
    [MenuItem ("Audio/Compression Bitrate (kbps)/64")]
    static void SetCompressionBitrate64kbps() {
        SelectedSetCompressionBitrate(64000);
    }
 
    [MenuItem ("Audio/Compression Bitrate (kbps)/96")]
    static void SetCompressionBitrate96kbps() {
        SelectedSetCompressionBitrate(96000);
    }
 
    [MenuItem ("Audio/Compression Bitrate (kbps)/128")]
    static void SetCompressionBitrate128kbps() {
        SelectedSetCompressionBitrate(128000);
    }
 
    [MenuItem ("Audio/Compression Bitrate (kbps)/144")]
    static void SetCompressionBitrate144kbps() {
        SelectedSetCompressionBitrate(144000);
    }
 
    [MenuItem ("Audio/Compression Bitrate (kbps)/156 (default)")]
    static void SetCompressionBitrate156kbps() {
        SelectedSetCompressionBitrate(156000);
    }
 
    [MenuItem ("Audio/Compression Bitrate (kbps)/160")]
    static void SetCompressionBitrate160kbps() {
        SelectedSetCompressionBitrate(160000);
    }
 
    [MenuItem ("Audio/Compression Bitrate (kbps)/192")]
    static void SetCompressionBitrate192kbps() {
        SelectedSetCompressionBitrate(192000);
    }
 
    [MenuItem ("Audio/Compression Bitrate (kbps)/224")]
    static void SetCompressionBitrate224kbps() {
        SelectedSetCompressionBitrate(224000);
    }
 
    [MenuItem ("Audio/Compression Bitrate (kbps)/240")]
    static void SetCompressionBitrate240kbps() {
        SelectedSetCompressionBitrate(240000);
    }
 
    // ----------------------------------------------------------------------------
 
    [MenuItem ("Audio/Load Type/Compressed in Memory")]
    static void ToggleDecompressOnLoadCompressedInMemory() {
        SelectedToggleDecompressOnLoadSettings(AudioImporterLoadType.CompressedInMemory);
    }
 
    [MenuItem("Audio/Load Type/Decompress on Load")]
    static void ToggleDecompressOnLoadDecompressOnLoad() {
        SelectedToggleDecompressOnLoadSettings(AudioImporterLoadType.DecompressOnLoad);
    }
 
    [MenuItem("Audio/Load Type/Stream from Disc")]
    static void ToggleDecompressOnLoadStreamFromDisc() {
        SelectedToggleDecompressOnLoadSettings(AudioImporterLoadType.StreamFromDisc);
    }
 
    // ----------------------------------------------------------------------------
 
    [MenuItem ("Audio/Toggle 3D Sound/Disable")]
    static void Toggle3DSoundDisable() {
        SelectedToggle3DSoundSettings(false);
    }
 
    [MenuItem ("Audio/Toggle 3D Sound/Enable")]
    static void Toggle3DSoundEnable() {
        SelectedToggle3DSoundSettings(true);
    }
 
    // ----------------------------------------------------------------------------
 
    [MenuItem ("Audio/Toggle Mono/Auto")]
    static void ToggleForceToMonoAuto() {
        SelectedToggleForceToMonoSettings(false);
    }
 
    [MenuItem ("Audio/Toggle Mono/Forced")]
    static void ToggleForceToMonoForced() {
        SelectedToggleForceToMonoSettings(true);
    }
 
    // ----------------------------------------------------------------------------
 
    static void SelectedToggleCompressionSettings(AudioImporterFormat newFormat) {
        AudioClip[] audioclips = GetSelectedAudioclips();
        foreach (AudioClip audioclip in audioclips) {
            string path = AssetDatabase.GetAssetPath(audioclip);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
            audioImporter.format = newFormat;
            AssetDatabase.ImportAsset(path);
        }
    }
 
    static void SelectedSetCompressionBitrate(int newCompressionBitrate) {
        AudioClip[] audioclips = GetSelectedAudioclips();
        foreach (AudioClip audioclip in audioclips) {
            string path = AssetDatabase.GetAssetPath(audioclip);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
            audioImporter.compressionBitrate = newCompressionBitrate;
            AssetDatabase.ImportAsset(path);
        }
    }
 
    static void SelectedToggleDecompressOnLoadSettings(AudioImporterLoadType loadType) {
        AudioClip[] audioclips = GetSelectedAudioclips();
        foreach (AudioClip audioclip in audioclips) {
            string path = AssetDatabase.GetAssetPath(audioclip);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
            audioImporter.loadType = loadType;
            AssetDatabase.ImportAsset(path);
        }
    }
 
    static void SelectedToggle3DSoundSettings(bool enabled) {
        AudioClip[] audioclips = GetSelectedAudioclips();
        foreach (AudioClip audioclip in audioclips) {
            string path = AssetDatabase.GetAssetPath(audioclip);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
            audioImporter.threeD = enabled;
            AssetDatabase.ImportAsset(path);
        }
    }
 
    static void SelectedToggleForceToMonoSettings(bool enabled) {
        AudioClip[] audioclips = GetSelectedAudioclips();
        foreach (AudioClip audioclip in audioclips) {
            string path = AssetDatabase.GetAssetPath(audioclip);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
            audioImporter.forceToMono = enabled;
            AssetDatabase.ImportAsset(path);
        }
    }
 
    static AudioClip[] GetSelectedAudioclips() {
        return Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets) as AudioClip[];
    }
}