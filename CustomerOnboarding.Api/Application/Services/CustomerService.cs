using AutoMapper;
using CustomerOnboarding.Api.Application.Interfaces;
using CustomerOnboarding.Api.Domain.Entities;
using CustomerOnboarding.Api.DTOs;
using Microsoft.AspNetCore.Identity;

public class CustomerService
{
private readonly IOtpService _otp;
private readonly IMapper _mapper;
private readonly ILogger<CustomerService> _logger;
private readonly PasswordHasher<Customer> _passwordHasher = new();
    private readonly ICustomerRepository _repo;


public CustomerService(ICustomerRepository repo, IOtpService otp, IMapper mapper, ILogger<CustomerService> logger)
{
    _repo = repo;
    _otp = otp;
    _mapper = mapper;
    _logger = logger;
}


public async Task StartOnboardingAsync(CreateCustomerRequest request, CancellationToken ct)
{
    // uniqueness checks
    if (await _repo.GetByPhoneAsync(request.PhoneNumber, ct) is not null)
        throw new InvalidOperationException("Phone number already in use.");
    if (await _repo.GetByEmailAsync(request.Email, ct) is not null)
        throw new InvalidOperationException("Email already in use.");


    // Ensure LGA belongs to State: this check is done in controller via DB context or repository; assume controller validates before calling


    // create customer (pending)
    var dummyCustomer = new Customer();
    var passwordHash = _passwordHasher.HashPassword(dummyCustomer, request.Password);
    var customer = Customer.Create(request.PhoneNumber, request.Email, passwordHash, request.StateId, request.LgaId);
    await _repo.AddAsync(customer, ct);


    // generate OTP and mock send
    await _otp.GenerateAndSendAsync(customer.PhoneNumber, ct);
    _logger.LogInformation("Started onboarding for {Phone}", customer.PhoneNumber);
}


public async Task<CustomerDto> VerifyOtpAndActivateAsync(VerifyOtpRequest request, CancellationToken ct)
{
    var customer = await _repo.GetByPhoneAsync(request.PhoneNumber, ct) ?? throw new KeyNotFoundException("Customer not found");
    if (customer.Status == CustomerStatus.Active) return _mapper.Map<CustomerDto>(customer);


    var ok = await _otp.VerifyAsync(request.PhoneNumber, request.Code, ct);
    if (!ok) throw new InvalidOperationException("Invalid or expired OTP");


    customer.Activate();
    await _repo.UpdateAsync(customer, ct);
    _logger.LogInformation("Customer {Phone} activated", customer.PhoneNumber);
    return _mapper.Map<CustomerDto>(customer);
}


public async Task<IEnumerable<CustomerDto>> GetAllAsync(CancellationToken ct)
{
    var list = await _repo.GetAllAsync(ct);
    return _mapper.Map<IEnumerable<CustomerDto>>(list.Where(c => c.Status == CustomerStatus.Active));
}
}