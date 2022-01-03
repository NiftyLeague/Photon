About
==================================================
This package contains runtime graphs and basic tools to help you with tracking overall game and Quantum simulation performance under various network conditions.
Graphs and their values are based on Unity update rate, single value in graph equals to time/count/... spent in single Unity frame.

Setup
==================================================
1) Drag & drop Prefabs/Profilers into your scene
2) Add EventSystem and InputModule if needed

Graphs
==================================================
1) Engine Delta Time - equals Time.unscaledDeltaTime between Unity frames
2) Frame Time        - all scripts logic including Unity internal + rendering, exclude wait for end of frame
3) User Scripts Time - all FixedUpdate() + Update() + LateUpdate()
4) Render Time       - equals time from last LateUpdate() till the end of render
5) Simulation Time   - equals QuantumRunner.Default.Game.Session.Stats.UpdateTime
6) Predicted Frames  - equals QuantumRunner.Default.Game.Session.PredictedFrames;
7) Verified Frames   - how many verified frames were simulated in particular Unity frame
8) Network Activity  - time since last data reveiced from server
9) Ping              - network peer round trip time
9) Markers           - you can track up to 8 custom boolean values using markers, each marker is represented by unique color (by default Red = input replaced by server, Orange = checksum calculated)

Simulation Tools
==================================================
Package also contains basic tools for changing target FPS (Application.targetFrameRate) and to simulate network conditions (lag, jitter, loss).
This is useful to quickly simulate different redering speed and bad networks with immediate effect in other graphs (predicted frames, simulation time, ...)

Notes
==================================================
Engine delta time sometimes doesn't reflect target FPS, to fix this you need to set QualitySettings.vSyncCount = 0;
When simulating Loss, set values carefully. Use 1-3% to simulate loss on network and higher values to simulate local loss (e.g. bad connection to router behind 3 walls)
Markers graph is running 2x faster for better readability (is controlled by Samples property on the Profilers prefab)
Multiple instances of MarkersProfiler are supported: 1) Get an instance by name MarkersProfiler profiler = MarkersProfiler.Get(GAMEOBJECT_NAME); 2) Call profiler.SetMarker(INDEX);
