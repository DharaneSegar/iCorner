// Top-level build file where you can add configuration options common to all sub-projects/modules.
buildscript {

        var compose_version = "1.4.3" // Declare your Compose version here
    
    repositories {
        google()
        mavenCentral()
    }
    dependencies {
        classpath("com.android.tools.build:gradle:8.0.2") // Use the appropriate version
        classpath("org.jetbrains.kotlin:kotlin-gradle-plugin:1.8.21")
    }
}

plugins {
    id("com.android.application") version "8.0.2" apply false
    id("org.jetbrains.kotlin.android") version "1.8.21" apply false
}
