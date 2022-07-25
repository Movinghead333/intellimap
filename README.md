# intellimap
Wave function collapse based tilemap generation in Unity3D editor.

## Installing the correct long term support version of the Unity Engine used for this project 
Editor version used: **2021.3.3f1**

First download the Unity Hub: [Unity Hub download](https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup.exe).

Then you can install the correct editor version via the following link: *unityhub://2021.3.3f1/af2e63e8f9bd*

## Usage
1. Go to the toolbar at the top of the Unity Editor window and select "Window > Intellimap".
2. Select a folder for the base data in the itellimap window, which is somewhere in a "Resources" directory and has the tilemap-prefabs you want to use in it.
3. Click the "Analyze Base Data" button to analyze the data within the tilemaps and load the data to the UI components. Reloading this data with the same path resets all changes made to analyzed data by overwriting it.
4. Play around with the UI elements for manipulating the data:
   1. When dragging a box in the matrix, the weights that influence probability of the column-tile appearing next to the row-tile are all simultaneously adjusted in proportion.
   2. When an element within the matrix is clicked, the underlying weights can be edited, each representing one cardinal direction (up, right, down, left). For example, the slider with the right arrow describes the weighting of the column-tile appearing to the right of the row-tile.
   3. There is also a histogram which gives the user control over the overall occurrences of tiles. Note however that due to the constraint propagation, probabilistic variance and the use of directional probabilities, this distribution is not necessarily reflected accurately in the final generated output.
5. Then select an output tilemap from your current Unity Scene and enter a width and height for the tilemap you want to generate.
6. When generating you have the option to tick the option "Use own seed" to have a fixed seed or enter a seed yourself. If this option is unchecked, a random seed based on the current time will be generated.
7. As a last step you can either generate the map in a single step with the "Generate Entire Map" button or have the generation be animated and toggled via the "Toggle Animated Generation" button. Lastly you can clear the generated output with the "Clear" button.
