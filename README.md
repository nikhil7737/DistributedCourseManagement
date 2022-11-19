<h1>OVERVIEW</h1>

This is a distributed course management application where instructors can add courses and students can enroll, unenroll or finish the courses. The API contract does sound simple but this is where the distributed nature of this application makes it slightly complex

This application has many individual services shown in the root folder. Together they are working on achieving this goal and implementing distributed patterns, namely

<ul>
    <li><b>Command and Query Responsibility Segregation (CQRS) with Snapshot</b></li>
    <li><b>Transactional Outbox Pattern</b></li>
    <li><b>Inbox Pattern</b></li>
</ul>

This application is deployed on AWS using services

<ul>
    <li><b>DynamoDB</b></li>
    <li><b>DynamoDB Streams</b></li>
    <li><b>Lambda</b></li>
    <li><b>API Gateway</b></li>
    <li><b>Eventbridge</b></li>
</ul>

It implements the transactional outbox pattern using <b>Change Data Capture</b> on DynamoDB using streams. Now follows the complete architecture.


<h1>Command Service</h2>

This is the command service where all the update requests are sent. It contains the APIs for both courses and enrollments

<h2>Course APIs</h2>

1. POST api/course: To create a new course. Takes the course body in input comprising of course name, fees, max students allowed to be enrolled, min enrollment duration, etc

2. DELETE api/course. To delete the course. course id is mandatory

Both of these endpoints at the end do these things

1. Create a CourseCreated or CourseDeleted event.
2. Creates an outbox entity containing just the aggregate id (an id shared among the events belonging to same entity, in this case course id), the event type (course event) and a unique id outboxId (GUI)
3. Persists these 2 entities in separate tables via a Dynamodb Transaction

<h2>Enrollment APIs</h2>

1. POST api/enrollment: To create or update an enrollment. A student might be enrolling, unenrolling or finishing a course. The payload contains studentId, courseId and what the student wants to do.

This endpoint at the end do these things

1. Create an Enrollment event.
2. Creates an outbox entity containing just the aggregate id (a combination of course and student id), the event type (enrollment event) and a unique id outboxId (GUI)
3. Persists these 2 entities in separate tables via a Dynamodb Transaction

Streams are enabled on outbox table which serves as <b>Change Data Capture</b> for a stream consumer


<h1>DDBStreamConsumer</h1>
This stream consumer deployed as a AWS lambda function is activated whenever there is a new entry in the outbox table.

1. It receives the outbox entity in the payload and figures out the event type (course or enrollment event)
2. Based on the event type, it sends the event to Eventbridge. There are 2 EB bus each for course and enrollment events
3. Since the events are always send to Eventbridge, <b>this ensures atleast once delivery</b>.


<h1>CoursesByStudentProjector</h1>

It consumes the enrollment event bus and populate the read models CourseByStudent which is the entity returned as a list when queried by studentId. This is also deployed as a AWS lambda function

1. The function gets triggered whenever there is a new message in the enrollment event bus. The message comprises of just the outbox 

2. It first checks if the message it received has already been processed and the message has just been delivered more than once. For that it checks if the message's unique identifier (outboxId) is present in a ProcessedOutboxEvent table. <b>If it is then the message is simply discarded making the consumer idempotent (exactly once processing)</b>

3. If the message is not processed, the latest snapshot is fetched from the snapshot table

4. Based on the snapshot's last processed event sequence number, all the later events are fetched from the enrollment table and applied one by one on the fetched snapshot. If no snapshot is present, an empty object is initialized

5. The app stores snapshots after every 3 events. So if a new snapshot is to be persited, a new one is created from the final read model

6. A transaction is initiated in which the updated read model, the snapshot (optional) and marking the event as processed are all committed together 


<h1>CoursesByInstructorProjector</h1>

It is almost similar to CoursesByStudentProjector except that it consumes course events and populates read model CourseByInstructor. This read model is used when GET requests are made to fetch courses by an instructor id.


<h1>QueryService</h1>

This service is the query part of the CQRS pattern. It exposes 2 types of APIs each for coursesByStudentId and coursesByInstructorId consuming their read models populatd their respective projectors.

1. GET query/courses/by/studentId/{studentId}: fetches courses a student is enrolled into
2. GET query/courses/by/instructorId/{instructorId}: fetches courses registered by an instructor
