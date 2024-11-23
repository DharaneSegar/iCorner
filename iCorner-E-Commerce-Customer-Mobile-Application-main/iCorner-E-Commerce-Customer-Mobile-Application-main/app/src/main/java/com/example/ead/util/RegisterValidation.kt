data class RegisterValidation(
    val email: ValidationResult = ValidationResult.Success,
    val password: ValidationResult = ValidationResult.Success,
    val confirmPassword: ValidationResult = ValidationResult.Success,
    val mobileNumber: ValidationResult = ValidationResult.Success
)

sealed class ValidationResult {
    object Success : ValidationResult()
    data class Failed(val message: String) : ValidationResult()
}
