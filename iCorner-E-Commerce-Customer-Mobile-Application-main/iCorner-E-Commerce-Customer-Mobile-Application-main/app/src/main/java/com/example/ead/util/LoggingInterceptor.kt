package com.example.ead.util


import okhttp3.Interceptor
import okhttp3.Response
import android.util.Log

class LoggingInterceptor : Interceptor {
    override fun intercept(chain: Interceptor.Chain): Response {
        val request = chain.request()

        Log.d("LoggingInterceptor", "Request URL: ${request.url}")
        Log.d("LoggingInterceptor", "Request Headers: ${request.headers}")

        val response = chain.proceed(request)

        Log.d("LoggingInterceptor", "Response Code: ${response.code}")
        Log.d("LoggingInterceptor", "Response Body: ${response.body?.string()}") // Note: Reading response body consumes it

        return response
    }
}
