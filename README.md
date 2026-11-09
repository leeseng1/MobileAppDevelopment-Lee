# MobileAppDevelopment-Lee

# Fitness Tracker API

Back end for my semester fitness app. Primary resource: workouts.

## Endpoints

| Method | URL | What it does | Success | Failure |
| --- | --- | --- | --- | --- |
| GET | /api/workouts | List all workouts; optional `?type=` filter | 200 | none |
| GET | /api/workouts/{id} | Get one workout | 200 | 404 |
| POST | /api/workouts | Create a workout | 201 (Location header) | 400, 409 |
| PUT | /api/workouts/{id} | Replace a workout | 200 | 400, 404, 409 |
| DELETE | /api/workouts/{id} | Delete a workout | 204 | 404 |
