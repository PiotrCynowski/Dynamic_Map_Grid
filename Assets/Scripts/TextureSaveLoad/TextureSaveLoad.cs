using GameMap.Generator;
using UnityEngine;
using UnityEngine.UI;

public class TextureSaveLoad : MonoBehaviour {
    Color fillColor = Color.white;

    public Image imageUI;

    private void Start() {
        Test();
    }

    public static TextureSaveLoad Instance;
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        }
        else {
            Instance = this;
        }
    }

    void Test() {
        Texture2D generatedTexture = GenerateTexture(64,64);
        SetPixelColor(generatedTexture, 10, 10, Color.red);
        imageUI.sprite = Sprite.Create(generatedTexture, new Rect(0, 0, generatedTexture.width, generatedTexture.height), Vector2.one * 0.5f);
    }

    public void Test2(int tileSize, float[,] map) {

        Texture2D generatedTexture = GenerateTexture(tileSize, tileSize);

        ModifyTextureFromFloatArray(generatedTexture, map, Color.magenta);
    }

    Texture2D GenerateTexture(int width, int height) {
        Texture2D texture = new Texture2D(width, height);
        // background color
        Color[] fillColorArray = new Color[texture.width * texture.height];
        for (int i = 0; i < fillColorArray.Length; i++) {
            fillColorArray[i] = fillColor;
        }
        texture.SetPixels(fillColorArray);
        texture.Apply(); 
        return texture;
    }

    void ModifyTextureFromFloatArray(Texture2D textureToModify, float[,] floatArray, Color pixelColor) {
        int width = textureToModify.width;
        int height = textureToModify.height;

        if (floatArray.GetLength(0) != width || floatArray.GetLength(1) != height) {
            Debug.LogError("Float array dimensions do not match texture dimensions.");
            return;
        }

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                SetPixelColor(textureToModify, x, y, pixelColor);
            }
        }

      
    }

    void SetPixelColor(Texture2D textureToModdify, int x, int y, Color color) {
        if (x >= 0 && x < textureToModdify.width && y >= 0 && y < textureToModdify.height) {
            textureToModdify.SetPixel(x, y, color);
            textureToModdify.Apply();
        }
        else {
            Debug.LogError("Invalid pixel coordinates or texture not generated yet.");
        }
    }

    float[,] GetPixelValues(Texture2D texture) {
        float[,] values = new float[texture.width, texture.height];
        Color pixelColor;
        for (int x = 0; x < texture.width; x++) {
            for (int y = 0; y < texture.height; y++) {
                pixelColor = texture.GetPixel(x, y);
                values[x, y] = pixelColor.grayscale; // Convert color to grayscale value (0.0f to 1.0f)
            }
        }
        return values;
    }

    Texture2D LoadTextureFromValues(float[,] values) {     
        Texture2D texture = new(values.GetLength(0), values.GetLength(1));

        for (int x = 0; x < texture.width; x++) {
            for (int y = 0; y < texture.height; y++) {           
                texture.SetPixel(x, y, new Color(values[x, y], values[x, y], values[x, y]));
            }
        }
        texture.Apply();
        return texture;
    }
}