package com.example.ead.util

import android.content.Context
import android.os.AsyncTask
import java.util.*
import javax.mail.Authenticator
import javax.mail.Message
import javax.mail.MessagingException
import javax.mail.PasswordAuthentication
import javax.mail.Session
import javax.mail.Transport
import javax.mail.internet.InternetAddress
import javax.mail.internet.MimeMessage

// Suppress the deprecation warning for AsyncTask, as it is deprecated in Android 11 (API 30)
@Suppress("DEPRECATION")
class EmailService(
    private val context: Context, // Context for accessing resources and services
    private val email: String,     // Recipient's email address
    private val subject: String,   // Subject of the email
    private val message: String     // Body of the email
) : AsyncTask<Void?, Void?, Void?>() {

    private var session: Session? = null // Variable to hold the email session

    // Background task to send the email
    override fun doInBackground(vararg params: Void?): Void? {
        // Properties to configure the SMTP server
        val properties = Properties().apply {
            put("mail.smtp.host", "smtp.gmail.com") // Set SMTP host
            put("mail.smtp.socketFactory.port", "465") // Set the socket factory port
            put("mail.smtp.socketFactory.class", "javax.net.ssl.SSLSocketFactory") // Use SSL
            put("mail.smtp.auth", "true") // Enable authentication
            put("mail.smtp.port", "465") // Set the SMTP port
        }

        // Create a new session with an authenticator
        session = Session.getDefaultInstance(properties, object : Authenticator() {
            // Provide email credentials for authentication
            override fun getPasswordAuthentication(): PasswordAuthentication {
                return PasswordAuthentication("nonamenecessary0612@gmail.com", "ekbgdpcvlpdiciws") // Change to secure method
            }
        })

        try {
            // Create a MimeMessage object for the email
            val mimeMessage = MimeMessage(session).apply {
                setFrom(InternetAddress("nonamenecessary0612@gmail.com")) // Set the sender's email address
                // Add recipient(s) to the email
                addRecipients(Message.RecipientType.TO, InternetAddress.parse(email))
                subject = this@EmailService.subject // Set the email subject
                setText(this@EmailService.message) // Set the email body
            }

            // Send the email
            Transport.send(mimeMessage)
        } catch (e: MessagingException) {
            // Print the stack trace if sending fails
            e.printStackTrace()
        }

        return null // Return null as no value is needed after execution
    }
}
