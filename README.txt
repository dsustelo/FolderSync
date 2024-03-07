# Folder Synchronizer

## Overview

The Folder Synchronizer is a simple command-line tool designed to synchronize directories between a source and a replica. It ensures that files and subdirectories are kept up-to-date across both locations.

## Usage

```
FolderSync.exe [SourcePath] [ReplicaPath] [SyncIntervalSeconds] [LogFilePath]
```

- **SourcePath:** The path to the source directory.
- **ReplicaPath:** The path to the replica directory.
- **SyncIntervalSeconds:** Time interval (in seconds) for synchronization.
- **LogFilePath:** Path to the log file for recording synchronization details.

## How to Run

1. Open a command prompt or terminal.
2. Navigate to the directory containing `FolderSync.exe`.
3. Run the command with appropriate arguments.

Example:

```
FolderSync.exe C:\SourceFolder C:\ReplicaFolder 5 C:\LogSync\log.txt
```

## Features

- Synchronizes files and subdirectories.
- Supports customizable synchronization intervals.
- Logs synchronization details to a specified log file.

## Dependencies

- .NET Framework

## Author

Diogo Sustelo