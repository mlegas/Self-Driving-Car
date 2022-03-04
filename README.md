# Self-Driving-Car
This repository contains the code and thesis PDF file for my master's dissertation, named _Implementing a one-way self-driving car framework using Unity ML-Agents and deep learning_.

In this project, I aimed to create an autonomous car in Unity that would be able to drive by itself on a one-way, one lane road and adjust its behaviour to a number of chosen traffic signs when driving. To achieve this, I decided to use [Unity's ML-Agents](https://unity.com/products/machine-learning-agents) for the reinforcement learning task, and the [German Traffic Sign Detection Benchmark](https://benchmark.ini.rub.de/gtsdb_news.html) dataset for the traffic sign detection part.

However, due to the outbreak of the COVID-19 pandemic in March 2020, access to suitable hardware to push forward the progress of this project has been severely limited, and therefore this project should be remarked as **unfinished**, and perhaps treated as a scrap car for parts.

## Project steps

Here I list what steps were taken during the development of this project, in order to explain the contents of each folder.

1. A player [car controller script](https://assetstore.unity.com/packages/tools/physics/ms-vehicle-system-free-version-90214) has been modified so that the RL agent could handle the inputs by itself.
2. A RL model was trained on a scene consisting of 6 agents driving on a straight lane, using a number of [raycasts](https://docs.unity3d.com/ScriptReference/Physics.Raycast.html) for observing the road around them. Unity ML-Agents's internal implementation of [PPO](https://openai.com/blog/openai-baselines-ppo/) was used to train the agents.
    * The car controller script, straight lane scene and trained model are all available in the `UnityProject/Self-DrivingCar/` directory.
3. A number of chosen German traffic signs have been textured for the traffic sign detection part, namely:
    * speed_limit_20
    * speed_limit_30
    * speed_limit_50
    * speed_limit_60
    * speed_limit_70
    * speed_limit_80
    * speed_limit_100
    * speed_limit_120
    * stop
    * restriction_ends
    * go_right
    * go_left
    * go_straight
4. 127 screenshots were made in Unity consisting of the traffic signs in an Unity scene.
5. These screenshots were further annotated and added to a copy of the GTSDB dataset.
    * The dataset can be found in the `SSD MobileNet Attempt/Full GTSDB Dataset + Unity Images (PNG)/` directory.
7. TFRecords files were created for [transfer learning](https://en.wikipedia.org/wiki/Transfer_learning) a SSD Mobilenet V2 with this dataset to be used further in the project.
    * Due to the size of the TFRecords files, they have been moved to an external file provider. The links can be found in the `SSD MobileNet Attempt/` directory.
8. A SSD Mobilenet V2 model was trained, unfortunately with 0% accuracy - this is possibly due to the size of the images that were later convoluted to a 320x240 image, and therefore unreadable for the CNN model.
    * Since the model has no further use, it has not been attached to this project.
    * However, the Python files used for training and the creation of TFRecords files can be found in the `SSD MobileNet Attempt/Used Python Code/` directory.
9. Due to this, I decided to use an already existing object detection model trained on the GTSDB dataset from [helloyide](https://github.com/helloyide/real-time-German-traffic-sign-recognition).
10. As Unity ML-Agents at the time of the creation of this project did not support using advanced models outside of Unity, 4 Unity gym environments were built, each with a rising difficulty level, to train the same model on each of these environments. This would facilitate the use of transfer learning fully.
    * These environments can be found in the `Built Unity Environments/` directory.
12. PPO2 and object detection scripts were being prepared to be used on these gym environments.
    * These scripts can be found in the `Used Python Code/` directory.
14. Project progress stopped.

## Documentation

Further information about the project can be found in the thesis provided as the `Thesis.pdf` file.
