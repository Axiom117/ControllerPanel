namespace KinematicSolver
{
    public class Solver
    {
        public JointAngles CalculateInverseKinematics(TargetPose target)
        {
            // implement IK here
            return 0;
        }
    }

    public struct TargetPose
    {
        public double X, Y, Z, Roll, Pitch, Yaw;
    }

    public struct JointAngles
    {
        public double Joint1, Joint2, Joint3, Joint4, Joint5, Joint6;
    }
}
