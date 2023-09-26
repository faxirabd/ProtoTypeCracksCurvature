# ProtoTypeCurvature_V3

This represents a prototype application designed to recognize cracks in glass and concrete RGB images using machine learning. Here's an overview of its key steps:
1-	Initially, individual frames or images extracted from a video undergo preprocessing, during which edge segments are isolated using the Edge Drawing (ED) algorithm.
2-	Following this, curvature features are derived from these edge segments, and a curvature histogram is generated.
3-	A Support Vector Machine (SVM) classifier is trained using this histogram to distinguish between images with cracks and those without cracks.
4-	Subsequently, the trained model is used to predict the class (cracked or non-cracked) of any new input images.

This prototype is implemented using C#.NET, built on WinForms, and developed in Visual Studio 2022. You can clone the repository onto your local computer and start the project using Visual Studio.


![Capture1](https://github.com/faxirabd/ProtoTypeCracksCurvature/assets/115953037/0f41ec38-0b2a-4a1b-beb8-aa2eee4221bc)


![Capture2](https://github.com/faxirabd/ProtoTypeCracksCurvature/assets/115953037/af7e0a44-8553-4d7c-beed-0489bb741ae7)
