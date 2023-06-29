# DanceIt

### Jeff's approach
To display the user content, I'm using a combination of ListView + Object Pooling.

The `ListView` already recycles it's elements to only use the necessary number of `Views` to fill the screen. In our case, even if 200 items get's added in the list, only between 1 and 3 visual elements will be kept in memory.

It also allows you to specify a `ListViewController`, containing the methods `BindItem` and `UnbindItem` which are callend whenever you need to repaint a element or hide/disable then.
This mechanism gets leveraged to dynamically enable/disable a `Character_Environment` in a Object Pooling pattern.

The `Character_Environment` is a simple combination of Camera, RenderTexture, and CharacterSpawnPosition. Used to render the character and display within the UI.

### Challenges
1) I had was to undestand how the `ListView` works as 100% of the projects I have participated so far decided to go with uGUI. Thankfully I already had experiences with it before, speeding up the discovery process.
2) `ListView` does not has a implementation to retrieve visible elements. I'm using this to implement the "jump to prev/next" behaviour.

### Future optimizations
I have not implemented a way to reuse the `ListView.itemSource` space. Meaning that it would grow internally as more data get's fetched. Apart from that, I have made sure to reuse common resources to maintain a stable user experience.

### Assets used

- [Mixamo](https://www.mixamo.com/)
- [139 Vector Icons](https://assetstore.unity.com/packages/2d/gui/icons/139-vector-icons-69968)
- [RPG Tiny Hero Duo PBR Polyart](https://assetstore.unity.com/packages/3d/characters/humanoids/rpg-tiny-hero-duo-pbr-polyart-225148)
- [Unity-Chan! Model](https://assetstore.unity.com/packages/3d/characters/unity-chan-model-18705)
- [Jammo Character | Mix and Jam](https://assetstore.unity.com/packages/3d/characters/jammo-character-mix-and-jam-158456)
- [Banana Man](https://assetstore.unity.com/packages/3d/characters/humanoids/banana-man-196830)
- [Casual 1 - Anime Girl Characters](https://assetstore.unity.com/packages/3d/characters/humanoids/casual-1-anime-girl-characters-185076)
