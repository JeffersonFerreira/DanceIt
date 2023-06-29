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
