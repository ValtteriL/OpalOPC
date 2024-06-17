# Billing

## Buying on invoice

1. Request the following info from customer:
    1. Full name
    2. Email address (for license delivery)
    3. Billing email address
    4. Currency
    5. Tax ID
    6. How many licenses
2. Create a new customer in [Stripe](https://dashboard.stripe.com/customers) with the info
3. Send invoice to the customer billing address
    * Make recurring if possible
4. Create license for the customer manually in [Keygen](https://app.keygen.sh/) and deliver it
5. Wait for the invoice to be paid
    * If failure to pay within period, disable license
